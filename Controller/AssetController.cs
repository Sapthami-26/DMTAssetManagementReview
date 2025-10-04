using DMTAssetManagement.Models;
using DMTAssetManagement.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DMTAssetManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetController : ControllerBase
    {
        private readonly IAssetRepository _assetRepository;

        public AssetController(IAssetRepository assetRepository)
        {
            _assetRepository = assetRepository;
        }

        // --- RETRIEVAL APIs ---

        // 1. Get Asset details 
        [HttpGet("{instanceId}")]
        public async Task<ActionResult<Asset>> GetAssetDetails(string instanceId)
        {
            var asset = await _assetRepository.GetAssetDetailsByInstanceIDAsync(instanceId);
            if (asset == null) return NotFound($"Asset details not found for Instance ID: {instanceId}");
            return Ok(asset);
        }
        
        // 2. Get Attachment Path 
        // FIX: The method now expects a List<Dictionary<string, object>> from the repository.
        [HttpGet("attachmentpath")]
        public async Task<IActionResult> GetAttachmentPath([FromQuery] string masterId, [FromQuery] string instanceId)
        {
            if (string.IsNullOrEmpty(masterId) || string.IsNullOrEmpty(instanceId))
            {
                return BadRequest("Missing MasterID or InstanceID query parameter.");
            }

            var dataList = await _assetRepository.GetAttachmentPathAsync(masterId, instanceId);

            // Check if the list is empty (equivalent to the old dataTable.Rows.Count == 0)
            if (dataList == null || dataList.Count == 0)
            {
                return NotFound($"Attachment path not found for MasterID: {masterId}, InstanceID: {instanceId}");
            }
            
            // Return the serializable list (this fixes the 500 error)
            return Ok(dataList); 
        }

        // 3. Get Asset Master ID 
        // FIX: The method now expects a List<Dictionary<string, object>> from the repository.
        [HttpGet("masterid/{masterId}/{instanceId}")]
        public async Task<IActionResult> GetAssetMasterID(string masterId, string instanceId)
        {
            var dataList = await _assetRepository.GetAssetMasterIDAsync(masterId, instanceId);

            if (dataList == null || dataList.Count == 0)
            {
                return NotFound($"Master ID data not found.");
            }
            
            // Return the serializable list (this fixes the 500 error)
            return Ok(dataList); 
        }

        // --- UPDATE APIs ---

        // 4. Update Asset Data (General Approval/Reject) 
        [HttpPut("approval/general")]
        public async Task<IActionResult> UpdateAssetGeneral([FromBody] AssetUpdate model)
        {
            if (model == null || string.IsNullOrEmpty(model.MasterID)) return BadRequest("Invalid approval data.");
            
            int rowsAffected = await _assetRepository.UpdateAssetDataAsync(model); 

            if (rowsAffected > 0)
            {
                return Ok(new { Message = "Asset data updated successfully (General Approval)." });
            }

            return BadRequest("Failed to update asset data (General Approval).");
        }

        // 5. Update Status on Inward (Inward Approval/Reject) 
        [HttpPut("approval/inward")]
        public async Task<IActionResult> UpdateAssetInward([FromBody] AssetUpdate model)
        {
            if (model == null || string.IsNullOrEmpty(model.MasterID)) return BadRequest("Invalid approval data.");
            
            int rowsAffected = await _assetRepository.UpdateStatusOnInwardAsync(model); 

            if (rowsAffected > 0)
            {
                return Ok(new { Message = "Asset status updated successfully (Inward Approval)." });
            }

            return BadRequest("Failed to update asset status (Inward Approval).");
        }

        // 6. Update Attachment Details (Metadata) 
        [HttpPost("attachment/metadata")]
        public async Task<IActionResult> UpdateAttachmentMetadata([FromQuery] string masterId, [FromQuery] string instanceId, [FromQuery] string fileName, [FromQuery] string filePath)
        {
            if (string.IsNullOrEmpty(masterId) || string.IsNullOrEmpty(instanceId) || string.IsNullOrEmpty(fileName))
            {
                return BadRequest("Missing required parameters for attachment metadata update.");
            }

            int rowsAffected = await _assetRepository.UpdateAttachmentDetailsAsync(masterId, instanceId, fileName, filePath);

            if (rowsAffected > 0)
            {
                return Ok(new { Message = "Attachment metadata updated successfully." });
            }

            return BadRequest("Failed to update attachment metadata.");
        }
        
        
        [HttpPost("attachment/uploadfile")]
        public async Task<IActionResult> UploadAttachmentFile([FromForm] FileUploadModel model) 
        {
            if (model == null || model.File == null || model.File.Length == 0)
            {
                return BadRequest("No file uploaded or missing form data.");
            }
            
            if (string.IsNullOrEmpty(model.MasterId) || string.IsNullOrEmpty(model.InstanceId))
            {
                return BadRequest("Missing MasterId or InstanceId in form data.");
            }
            
            return Ok(new { Message = $"File {model.File.FileName} received. Metadata must be updated separately using the /attachment/metadata endpoint." });
        }
    }
}