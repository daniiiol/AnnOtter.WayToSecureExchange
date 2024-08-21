using AnnOtter.WayToSecureExchange.Configuration.AppSettings;
using AnnOtter.WayToSecureExchange.Exceptions.Exchange;
using AnnOtter.WayToSecureExchange.Helpers;
using AnnOtter.WayToSecureExchange.Models.API.Exchange;
using AnnOtter.WayToSecureExchange.Models.Database;
using AnnOtter.WayToSecureExchange.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AnnOtter.WayToSecureExchange.Controllers.API
{
    /// <summary>
    /// Web API to handle a secure exchange.
    /// </summary>
    [Route("api/exchange")]
    [ApiController]
    public class ExchangeApiController : ControllerBase
    {
        private readonly ISecretEntityRepository _secretRepo;
        private readonly ILogger _logger;
        private readonly IOptions<EncryptionSettings> _config;

        /// <summary>
        /// Ctor of Web API Controller.
        /// </summary>
        /// <param name="logger">DI injected logging framework.</param>
        /// <param name="secretRepository">DI injected repository to manage the database.</param>
        /// <param name="configuration">DI injected EncryptionSettings instance.</param>
        public ExchangeApiController(ILogger<ExchangeApiController> logger, ISecretEntityRepository secretRepository, IOptions<EncryptionSettings> configuration)
        {
            this._logger = logger;
            this._secretRepo = secretRepository;
            this._config = configuration;
        }

        /// <summary>
        /// Endpoint to check if a secret exists.
        /// </summary>
        /// <param name="data">ID to identify a secret uniquely.</param>
        /// <returns>If successful, transmits HTTP status code 200 (ok); if an error occurs, transmits 500 (server error) or 404 (not found).</returns>
        [HttpHead]
        public StatusCodeResult Head(string data)
        {
            if(string.IsNullOrEmpty(data))
            {
                NotFound();
            }

            try
            {
                if(!Guid.TryParse(data, out var id))
                {
                    return NotFound();
                }

                var result = _secretRepo.ExistsSecret(id);

                if(result)
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex.Message}", ex.Message);
                return StatusCode(500);
            }

            return NotFound();
        }

        /// <summary>
        /// Endpoint to receive a secret once. After successful transmission the secret is deleted directly.
        /// </summary>
        /// <param name="data">ID to identify a secret uniquely.</param>
        /// <returns>If successful, transmits HTTP status code 200 (ok) with the data object; if an error occurs, transmits 500 (server error) or 404 (not found).</returns>
        [HttpGet]
        public ActionResult<DownloadModel> Get(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                NotFound();
            }
            
            try
            {
                if (!Guid.TryParse(data, out var id))
                {
                    return NotFound();
                }

                var result = _secretRepo.GetSecretById(id);

                if (result != null)
                {
                    var key = _config.Value.Key;
                    var plaintext = result.Ciphertext;
                    var tag = result.Tag;

                    if (!string.IsNullOrEmpty(result.Nonce) && !string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(tag))
                    {
                        // Decrypt the double-encrypted database object
                        plaintext = CryptoHelper.DecryptChaCha20Poly1305(result.Ciphertext, result.Nonce, key, tag);
                    }

                        var downloadModel = new DownloadModel
                    {
                        UploadId = result.SecretId.ToString(),
                        Data = plaintext,
                        Hash = CryptoHelper.GetSha256Hash(plaintext)
                    };

                    // Delete the secret
                    _secretRepo.DeleteSecret(id);

                    return Ok(downloadModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex.Message}", ex.Message);
                return StatusCode(500);
            }

            return NotFound();
        }

        /// <summary>
        /// Uploads the ciphertext to the database.
        /// </summary>
        /// <param name="data">ID to identify a secret uniquely.</param>
        /// <returns>Confirmation of the upload process. If successful, transmits HTTP status code 200 (ok) with a confirmation object; if an error occurs, transmits 500 (server error).</returns>
        [HttpPost("upload")]
        public ActionResult<UploadConfirmationModel> Upload([FromBody] UploadDataModel uploadData)
        {
            try
            {
                var data = (uploadData != null && !string.IsNullOrEmpty(uploadData.Data)) ? uploadData.Data : string.Empty;

                if (data == null || string.IsNullOrEmpty(data))
                {
                    throw new InputDataInvalidException("Validation Error: data must not be empty.");
                }
                else if (data.Length > 4096)
                {
                    throw new InputDataInvalidException("Validation Error: data must not contain more than 2000 characters.");
                }

                var resultModel = new UploadConfirmationModel();

                var nonce = string.Empty;
                var tag = string.Empty;

                var secretEntity = new SecretEntity()
                {
                    Ciphertext = data,
                    CreatedDate = DateTime.UtcNow,
                    Nonce = nonce,
                    Tag = tag
                };

                var key = _config.Value.Key;                
                var newCiphertext = string.Empty;
                
                if(!string.IsNullOrEmpty(key)) 
                {
                    // Double-encrypt the secret with ChaCha20Poly1305
                    var result = CryptoHelper.EncryptChaCha20Poly1305(data, key);
                    nonce = result.Nonce;
                    newCiphertext = result.Ciphertext;
                    tag = result.Tag;
                }
                
                if (!string.IsNullOrEmpty(nonce) && !string.IsNullOrEmpty(newCiphertext)) 
                {
                    secretEntity.Ciphertext = newCiphertext;
                    secretEntity.Nonce = nonce;
                    secretEntity.Tag = tag;
                }               

                var dbSecretEntity = _secretRepo.AddSecret(secretEntity);

                resultModel.UploadState = UploadState.Succeeded;
                resultModel.UploadTimestamp = dbSecretEntity.CreatedDate;
                resultModel.UploadMessage = "Upload successful.";
                resultModel.UploadId = dbSecretEntity.SecretId.ToString();
                resultModel.UploadOriginHash = CryptoHelper.GetSha256Hash(data);

                return resultModel;
            }
            catch (InputDataInvalidException ex)
            {
                _logger.LogError("{ex.Message}", ex.Message);
                return CreateExceptionUploadModel(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex.Message}", ex.Message);
                return CreateExceptionUploadModel("Unknown issue. Please try again later.");
            }
        }

        /// <summary>
        /// Creates an ExceptionUploadModel for exception management.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>A negative confirmation of secret upload.</returns>
        private static UploadConfirmationModel CreateExceptionUploadModel(string message)
        {
            var resultModel = new UploadConfirmationModel
            {
                UploadId = string.Empty,
                UploadTimestamp = DateTime.Now,
                UploadState = UploadState.Failed,
                UploadMessage = message
            };

            return resultModel;
        }
    }

}
