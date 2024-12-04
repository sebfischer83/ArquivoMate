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
            logger.LogDebug(processDocumentCommand.ToString());
            string filePath = processDocumentCommand.DocumentPath;
            if (!File.Exists(filePath))
            {
                logger.LogError($"File not found: {filePath}");
                var message = new HubResponse<HubResponseProgressData>();
                message.SetErrorResponse("File not found");
                //await communicationHub.SendDocumentStatus(userService.GetUserName()!, processDocumentCommand.DocumentId.ToString(), message);

                return;
            }
            byte[] fileData = System.IO.File.ReadAllBytes(filePath);

            // dateitypen bestimmen
            var fileType = Path.GetExtension(processDocumentCommand.DocumentName).ToLower();

            if (DocumentProcessorVariables.ImageExtensions.Contains(fileType))
            {
                await ProcessImageAsync(processDocumentCommand, fileData);
            }
            else if (DocumentProcessorVariables.OfficeExtensions.Contains(fileType))
            {
                await ProcessOfficeDocumentAsync(processDocumentCommand, fileData);
            }
            else if (DocumentProcessorVariables.TextExtensions.Contains(fileType))
            {
                await ProcessTextDocumentAsync(processDocumentCommand, fileData);
            }
            else if (fileType == ".pdf")
            {
                await ProcessPdfDocumentAsync(processDocumentCommand, fileData);
            }
            else
            {
                logger.LogError($"Unsupported file type: {fileType}");
                var message = new HubResponse<HubResponseProgressData>();
                message.SetErrorResponse("Unsupported file type");
               ///* await communicationHub.SendDocumentStatus(userService.GetUserName()!, processDocumentCo*/mmand.DocumentId.ToString(), message);
            }
        }

        private async Task ProcessPdfDocumentAsync(ProcessDocumentCommand processDocumentCommand, byte[] fileData)
        {
            await fileService.WriteAsync($"data/twst/nez/pdf.pdf", "application/pdf", fileData);

            // create image from pdf
            var orgImagePng = GeneratePdfImage(fileData);
            // create thumbnail
            var thumbnailWebp = GenerateThumbnail(orgImagePng);
            // get text from pdf
            await OcrPdf(fileData);

            // save original file
            // save thumbnail
            // save converted pdf
            // save data to database
        }

        private async Task OcrPdf(byte[] fileData)
        {
            using var client = httpClientFactory.CreateClient("ocrmypdf");
            using var form = new MultipartFormDataContent();
            using var fileContent = new StreamContent(new MemoryStream(fileData));
            
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
            form.Add(fileContent, "file", "file.pdf");

            var response = await client.PostAsync("/Ocrmypdf/Convert", form);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("File uploaded successfully!");
                var jsonString = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<OcrPdfResponse>(jsonString);
            }
            else
            {
                Console.WriteLine("File upload failed!");
            }
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

        private async Task ProcessTextDocumentAsync(ProcessDocumentCommand processDocumentCommand, byte[] fileData)
        {
            throw new NotImplementedException();
        }

        private async Task ProcessOfficeDocumentAsync(ProcessDocumentCommand processDocumentCommand, byte[] fileData)
        {
            throw new NotImplementedException();
        }

        private async Task ProcessImageAsync(ProcessDocumentCommand processDocumentCommand, byte[] fileData)
        {
            throw new NotImplementedException();
        }

        private byte[] GeneratePdfImage(byte[] fileData)
        {
            string pathOrgFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".pdf");
            string pathImageFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            File.WriteAllBytes(pathOrgFile, fileData);

            string command = "pdftoppm";

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = command;
            startInfo.Arguments = $"-png -f 1 -singlefile {pathOrgFile} {pathImageFile}";
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
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
            var b = File.ReadAllBytes(pathImageFile);
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
}
