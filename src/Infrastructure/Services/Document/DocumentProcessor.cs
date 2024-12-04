using Amazon.Runtime;
using ArquivoMate.Application;
using ArquivoMate.Application.Commands.Document;
using ArquivoMate.Application.Interfaces;
using ArquivoMate.Infrastructure.Data;
using ArquivoMate.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ArquivoMate.Infrastructure.Services.Document
{
    public class DocumentProcessor : IDocumentProcessor
    {
        private readonly ILogger<DocumentProcessor> logger;
        private readonly ArquivoMateDbContext dbContext;
        private readonly ICommunicationHub communicationHub;
        private readonly IUserService userService;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IFileService fileService;

        public DocumentProcessor(ILogger<DocumentProcessor> logger,
            ArquivoMateDbContext dbContext, ICommunicationHub communicationHub, IUserService userService,
            IHttpClientFactory httpClientFactory, IFileService fileService)
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.communicationHub = communicationHub;
            this.userService = userService;
            this.httpClientFactory = httpClientFactory;
            this.fileService = fileService;
        }

        public async Task ProcessDocument(ProcessDocumentCommand processDocumentCommand)
        {
            logger.LogInformation($"Start processing {processDocumentCommand.ToString()}");
            string filePath = processDocumentCommand.DocumentPath;
            if (!File.Exists(filePath))
            {
                logger.LogError($"File not found: {filePath}");
                var message = new HubResponse<HubResponseProgressData>();
                message.SetErrorResponse("File not found");
                await communicationHub.SendDocumentStatus(userService.GetUserName()!, processDocumentCommand.DocumentId.ToString(), message);

                return;
            }
            byte[] fileData = File.ReadAllBytes(filePath);
            logger.LogDebug($"Processing {filePath} with size {fileData.Length} bytes");

            // dateitypen bestimmen
            var fileType = Path.GetExtension(processDocumentCommand.DocumentName).ToLower();
            logger.LogDebug($"Found file type {fileType}");

            DocumentProcessorConvertStepResult? convertStepResult = null;
            await ConvertDocument(processDocumentCommand.DocumentId, fileData, fileType, convertStepResult);

            logger.LogInformation($"Completed converting step with result {convertStepResult}");

            if (convertStepResult == null || !convertStepResult.IsSuccess)
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
                logger.LogError($"Convert of file was not succesfull {filePath} {processDocumentCommand.DocumentId}");
                var message = new HubResponse<HubResponseProgressData>();
                message.SetErrorResponse("Error while converting file");
                await communicationHub.SendDocumentStatus(userService.GetUserName()!, processDocumentCommand.DocumentId.ToString(), message);
                return;
            }

            var userId = userService.GetUserId();
            var recordId = Guid.NewGuid();

            logger.LogInformation($"Identity for new document will be {recordId} for user {userId}");

        }

        private async Task ConvertDocument(Guid documentId, byte[] fileData, string fileType, DocumentProcessorConvertStepResult? convertStepResult)
        {
            if (DocumentProcessorVariables.ImageExtensions.Contains(fileType))
            {
                await ProcessImageAsync(fileData);
            }
            else if (DocumentProcessorVariables.OfficeExtensions.Contains(fileType))
            {
                await ProcessOfficeDocumentAsync(fileData);
            }
            else if (DocumentProcessorVariables.TextExtensions.Contains(fileType))
            {
                await ProcessTextDocumentAsync(fileData);
            }
            else if (fileType == ".pdf")
            {
                convertStepResult = await ProcessPdfDocumentAsync(fileData);
            }
            else
            {
                logger.LogError($"Unsupported file type: {fileType}");
                var message = new HubResponse<HubResponseProgressData>();
                message.SetErrorResponse("Unsupported file type");
                await communicationHub.SendDocumentStatus(userService.GetUserName()!, documentId.ToString(), message);
            }
        }

        private async Task<DocumentProcessorConvertStepResult>
            ProcessPdfDocumentAsync(byte[] fileData)
        {
            logger.LogInformation("Processing pdf file");
            // create image from pdf
            var orgImagePng = GeneratePdfImage(fileData);
            // create thumbnail
            var thumbnailWebp = GenerateThumbnail(orgImagePng);
            // get text from pdf
            var responseOcr = await OcrPdf(fileData);

            if (orgImagePng == null || thumbnailWebp == null || responseOcr == null)
            {
                logger.LogError("Error processing pdf file");
                return new DocumentProcessorConvertStepResult() { IsSuccess = false };
            }

            return new DocumentProcessorConvertStepResult()
            {
                IsSuccess = true,
                Image = orgImagePng,
                Thumbnail = thumbnailWebp,
                Content = responseOcr.Content,
                GeneratedPdf = responseOcr.GeneratedPdf
            };
        }

        private async Task<OcrPdfResponse?> OcrPdf(byte[] fileData)
        {
            using var client = httpClientFactory.CreateClient("ocrmypdf");
            using var form = new MultipartFormDataContent();
            using var fileContent = new StreamContent(new MemoryStream(fileData));

            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
            form.Add(fileContent, "file", "file.pdf");

            var response = await client.PostAsync("/Ocrmypdf/Convert", form);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<OcrPdfResponse>(jsonString);
                return responseObject;
            }
            else
            {
                Console.WriteLine("File upload failed!");
            }
            return null;
        }

        private byte[] GenerateThumbnail(byte[] orgImageBytes)
        {
            var orgImage = Image.Load(orgImageBytes);

            int thumbnailWidth = 300; // Set the desired width of the thumbnail
            int thumbnailHeight = 300; // Set the desired height of the thumbnail

            var thumbnail = orgImage.Clone(ctx => ctx.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(thumbnailWidth, thumbnailHeight)
            }));

            using MemoryStream memoryStream = new MemoryStream();
            thumbnail.SaveAsWebp(memoryStream);

            return memoryStream.ToArray();
        }

        private async Task ProcessTextDocumentAsync(byte[] fileData)
        {
            throw new NotImplementedException();
        }

        private async Task ProcessOfficeDocumentAsync(byte[] fileData)
        {
            throw new NotImplementedException();
        }

        private async Task ProcessImageAsync(byte[] fileData)
        {
            throw new NotImplementedException();
        }

        private byte[] GeneratePdfImage(byte[] fileData)
        {
            string pathOrgFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".pdf");
            string pathImageFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            logger.LogDebug($"Convert pdf to image, pdf {pathOrgFile} to image {pathImageFile}");

            File.WriteAllBytes(pathOrgFile, fileData);

            string command = "pdftoppm";

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = command;
            startInfo.Arguments = $"-png -f 1 -singlefile {pathOrgFile} {pathImageFile}";
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            logger.LogDebug($"Execute console with {startInfo.FileName} {startInfo.Arguments}");

            using (Process process = new Process())
            {
                process.StartInfo = startInfo;

                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string errors = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    logger.LogError(errors);
                }

                logger.LogDebug(output);
                logger.LogDebug(errors);

            }
            pathImageFile += ".png";

            if (File.Exists(pathImageFile))
            {
                logger.LogDebug($"Image file created {pathImageFile}");
            }
            else
            {
                logger.LogError($"Error creating image file {pathImageFile}");
                return [];
             }

            var b = File.ReadAllBytes(pathImageFile);

            logger.LogDebug($"Image file size {b.Length}");

            try
            {
                File.Delete(pathOrgFile);
                File.Delete(pathImageFile);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting temp files");
            }
            return b;
        }
    }

    internal class DocumentProcessorConvertStepResult
    {
        public bool IsSuccess { get; set; }
        public byte[]? Image { get; set; }
        public byte[]? Thumbnail { get; set; }
        public string? Content { get; set; }
        public byte[]? GeneratedPdf { get; set; }
                public override string ToString()
        {
            return $"IsSuccess: {IsSuccess}, Image: {Image?.Length}, Thumbnail: {Thumbnail?.Length}, Content: {Content?.Length}, GeneratedPdf: {GeneratedPdf?.Length}";
        }
    }
}
