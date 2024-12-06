using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ArquivoMate.Shared
{
    public class HubResponse<T>
    {
        public bool IsSucceed { get; private set; } = true;
        public string ErrorMessage { get; private set; } = "";
        public T? Data { get; private set; }

        public HubResponse<T> SetSuccessResponse(T data)
        {
            Data = data;
            return this;
        }

        public HubResponse<T> SetErrorResponse(string message)
        {
            IsSucceed = false;
            ErrorMessage = message;
            return this;
        }
    }

    public class DocumentProcessingFinishedData
    {
        public string Message { get; set; }
    }

    public class AppResponse<T>
    {
        public bool IsSucceed { get; private set; } = true;
        public Dictionary<string, string[]> Messages { get; private set; } = [];

        public T? Data { get; private set; }
        public AppResponse<T> SetSuccessResponse(T data)
        {
            Data = data;
            return this;
        }
        public AppResponse<T> SetSuccessResponse(T data, string key, string value)
        {
            Data = data;
            Messages.Add(key, [value]);
            return this;
        }
        public AppResponse<T> SetSuccessResponse(T data, Dictionary<string, string[]> message)
        {
            Data = data;
            Messages = message;
            return this;
        }
        public AppResponse<T> SetSuccessResponse(T data, string key, string[] value)
        {
            Data = data;
            Messages.Add(key, value);
            return this;
        }
        public AppResponse<T> SetErrorResponse(string key, string value)
        {
            IsSucceed = false;
            Messages.Add(key, [value]);
            return this;
        }
        public AppResponse<T> SetErrorResponse(string key, string[] value)
        {
            IsSucceed = false;
            Messages.Add(key, value);
            return this;
        }
        public AppResponse<T> SetErrorResponse(Dictionary<string, string[]> message)
        {
            IsSucceed = false;
            Messages = message;
            return this;
        }
    }
}
