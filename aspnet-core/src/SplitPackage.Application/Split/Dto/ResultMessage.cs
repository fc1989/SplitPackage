using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SplitPackage.Split.Dto
{
    public class ResultMessage<T>
    {
        public ResultCode Code { get; set; }

        public string Message { get; set; }

        public T Result { get; set; }

        public ResultMessage(ResultCode resultCode, string resultMsg, T obj)
        {
            this.Message = resultMsg;
            this.Code = resultCode;
            this.Result = obj;
        }

        public ResultMessage(ResultCode resultCode, string resultMsg)
        {
            this.Message = resultMsg;
            this.Code = resultCode;
        }
    }

    public enum ResultCode
    {
        Success = 200,
        BadRequest = 400,
        Auth_Error = 401,
        NoFind = 404,
        UnsupportedMediaType = 415,
        SytemError = 500,

        BadRequest_ParamEmpty = 40001,
        BadRequest_ParamConstraint = 40002,

        Auth_InvalidToken = 40100,
        Auth_InvalidAutheHeader = 40101,
        Auth_InvalidInput = 40102,
        Auth_RefuseAuthorization = 40103,
    }
}