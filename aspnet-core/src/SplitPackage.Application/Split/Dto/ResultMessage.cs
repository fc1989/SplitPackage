using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SplitPackage.Split.Dto
{
    /// <summary>
    /// 返回类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultMessage<T>
    {
        private int _code = 0;
        private string _message = string.Empty;
        private T _data = default(T);

        public int Code
        {
            get { return _code; }
            set { _code = value; }
        }

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public T Result
        {
            get { return _data; }
            set { _data = value; }
        }

        public ResultMessage(int resultCode, string resultMsg, T data)
        {
            _message = resultMsg;
            _code = resultCode;
            _data = data;
        }
        public ResultMessage(int resultCode, string resultMsg)
        {
            _message = resultMsg;
            _code = resultCode;
        }
    }

    public class ResultConfig
    {
        public static Dictionary<ResultCode, string> Configs = new Dictionary<ResultCode, string>() {
            { ResultCode.Success, "操作成功"},
            { ResultCode.KeyIsNull, "key值为空"},
            { ResultCode.KeyIsNotExist, "key值不存在"},
            { ResultCode.InternalServerError, "服务器异常"},
            { ResultCode.MysqlException, "数据库操作异常"},
            { ResultCode.ContentTypeNotCorrect, "媒体类型不是\"application/json\""}
        };
    }

    public enum ResultCode
    {
        Success = 200,
        KeyIsNull = 40001,
        KeyIsNotExist = 40002,
        InternalServerError = 40003,
        MysqlException = 40004,
        ContentTypeNotCorrect = 40005
    }
}