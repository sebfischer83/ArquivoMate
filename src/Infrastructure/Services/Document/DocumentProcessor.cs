using ArquivoMate.Application;
using ArquivoMate.Application.Commands.Document;
using ArquivoMate.Application.Interfaces;
using ArquivoMate.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Infrastructure.Services.Document
{
    public class DocumentProcessor : IDocumentProcessor
    {

        private readonly ILogger<DocumentProcessor> logger;
        private readonly ArquivoMateDbContext dbContext;
        private readonly ICommunicationHub communicationHub;
        private readonly IUserService userService;

        public static readonly HashSet<string> ImageExtensions = new HashSet<string>
        {
            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".tif", ".webp", ".svg", ".heic", ".heif", ".ico"
        };

        public static readonly HashSet<string> OfficeExtensions = new HashSet<string>
        {
            ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
            ".odt", ".ods", ".odp",                              
            ".rtf", ".txt", ".csv",                              
        };

        public static readonly HashSet<string> TextExtensions = new HashSet<string>
        {
            ".rtf", ".txt", ".csv",
        };

        public DocumentProcessor(ILogger<DocumentProcessor> logger,
            ArquivoMateDbContext dbContext, ICommunicationHub communicationHub, IUserService userService)
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.communicationHub = communicationHub;
            this.userService = userService;
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
                await communicationHub.SendDocumentStatus(userService.GetUserName()!, processDocumentCommand.DocumentId.ToString(), message);

                return;
            }
            byte[] fileData = System.IO.File.ReadAllBytes(filePath);

            // dateitypen bestimmen
            var fileType = Path.GetExtension(filePath).ToLower();

            if (ImageExtensions.Contains(fileType))
            {
                await ProcessImageAsync(processDocumentCommand, fileData);
            }
            else if (OfficeExtensions.Contains(fileType))
            {
                await ProcessOfficeDocumentAsync(processDocumentCommand, fileData);
            }
            else if (TextExtensions.Contains(fileType))
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
                await communicationHub.SendDocumentStatus(userService.GetUserName()!, processDocumentCommand.DocumentId.ToString(), message);
            }
        }

        private async Task ProcessPdfDocumentAsync(ProcessDocumentCommand processDocumentCommand, byte[] fileData)
        {
            // create thumbnail from pdf
            // get text from pdf
            // save original file
            // save thumbnail
            // save converted pdf
            // save data to database
            throw new NotImplementedException();
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
    }
}
