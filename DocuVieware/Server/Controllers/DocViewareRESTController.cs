using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using DocuVieware.Shared;
using GdPicture14.WEB;
using DocuVieware.Client.Shared;

namespace DocuVieware.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocViewareRESTController : ControllerBase
    {
        [HttpPost]
        [Route("GetDocuViewareControl")]
        public IActionResult GetDocuViewareControl([FromBody] DocuViewarConfiguration controlConfiguration)
        {
            if (!DocuViewareManager.IsSessionAlive(controlConfiguration.SessionId))
            {
                if (!string.IsNullOrEmpty(controlConfiguration.SessionId) && !string.IsNullOrEmpty(controlConfiguration.ControlId))
                {
                    DocuViewareManager.CreateDocuViewareSession(controlConfiguration.SessionId,
                        controlConfiguration.ControlId, 20);
                }
                else
                {
                    throw new Exception("Invalid session identifier and/or invalid control identifier.");
                }
            }
            using var docuVieware = new DocuViewareControl(controlConfiguration.SessionId)
            {
                AllowPrint = controlConfiguration.AllowPrint,
                EnablePrintButton = controlConfiguration.EnablePrintButton,
                AllowUpload = controlConfiguration.AllowUpload,
                EnableFileUploadButton = controlConfiguration.EnableFileUploadButton,
                CollapsedSnapIn = controlConfiguration.CollapsedSnapIn,
                ShowAnnotationsSnapIn = controlConfiguration.ShowAnnotationsSnapIn,
                EnableRotateButtons = controlConfiguration.EnableRotateButtons,
                EnableZoomButtons = controlConfiguration.EnableZoomButtons,
                EnablePageViewButtons = controlConfiguration.EnablePageViewButtons,
                EnableMultipleThumbnailSelection = controlConfiguration.EnableMultipleThumbnailSelection,
                EnableMouseModeButtons = controlConfiguration.EnableMouseModeButtons,
                EnableFormFieldsEdition = controlConfiguration.EnableFormFieldsEdition,
                EnableTwainAcquisitionButton = controlConfiguration.EnableTwainAcquisitionButton,
                MaxUploadSize = 36700160 // 35MB
            };
            using StringWriter controlOutput = new StringWriter();
            docuVieware.RenderControl(controlOutput);
            return new OkObjectResult(new DocuViewareRESTOutputResponse
            {
                HtmlContent = controlOutput.ToString()
            });
        }
        [HttpGet("ping")]
        public string ping()
        {
            return "pong";
        }
        [HttpPost("baserequest")]
        public string baserequest([FromBody] object jsonString)
        {
            return DocuViewareControllerActionsHandler.baserequest(jsonString);
        }


        [HttpGet("print")]
        public HttpResponseMessage Print(string sessionID, string pageRange, bool printAnnotations)
        {
            return DocuViewareControllerActionsHandler.print(sessionID, pageRange, printAnnotations);
        }

        [HttpGet("save")]
        public IActionResult Save(string sessionID, string fileName, string format, string pageRange, bool dropAnnotations, bool flattenAnnotations)
        {
            DocuViewareControllerActionsHandler.save(sessionID, ref fileName, format, pageRange, dropAnnotations, flattenAnnotations, out HttpStatusCode statusCode, out string reasonPhrase, out byte[] content, out string contentType);
            if (statusCode == HttpStatusCode.OK)
            {
                return File(content, contentType, fileName);
            }
            else
            {
                return StatusCode((int)statusCode, reasonPhrase);
            }
        }


        [HttpGet("twainservicesetupdownload")]
        public IActionResult TwainServiceSetupDownload(string sessionID)
        {
            DocuViewareControllerActionsHandler.twainservicesetupdownload(sessionID, out HttpStatusCode statusCode, out byte[] content, out string contentType, out string fileName, out string reasonPhrase);
            if (statusCode == HttpStatusCode.OK)
            {
                return File(content, contentType, fileName);
            }
            else
            {
                return StatusCode((int)statusCode, reasonPhrase);
            }
        }

        [HttpPost("formfieldupdate")]
        public string FormfieldUpdate([FromBody] object jsonString)
        {
            return DocuViewareControllerActionsHandler.formfieldupdate(jsonString);
        }

        [HttpPost("annotupdate")]
        public string AnnotUpdate([FromBody] object jsonString)
        {
            return DocuViewareControllerActionsHandler.annotupdate(jsonString);
        }

        [HttpPost("loadfromfile")]
        public string LoadFromFile([FromBody] object jsonString)
        {
            return DocuViewareControllerActionsHandler.loadfromfile(jsonString);
        }

        [HttpPost("loadfromfilemultipart")]
        public string LoadFromFileMultipart()
        {
            return DocuViewareControllerActionsHandler.loadfromfilemultipart(Request);
        }
    }
}
   

