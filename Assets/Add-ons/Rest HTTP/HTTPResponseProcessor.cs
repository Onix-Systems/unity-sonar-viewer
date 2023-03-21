
using UnityEngine;
using Newtonsoft.Json;
using System;

namespace RestHTTP
{
    public static class HTTPResponseProcessor
    {
        public static Result GetResult(HTTPResponse httpResponse)
        {
            Result responseResult = new Result();

            if (httpResponse == null)
            {
                responseResult.IsSuccess = false;
                responseResult.Message = "Unknown request error occurred";
                responseResult.StatusCode = 0;

                return responseResult;
            }

            responseResult.Aborted = httpResponse.RequestAborted;
            responseResult.IsSuccess = httpResponse.IsSuccess;
            responseResult.Message = httpResponse.Error;
            responseResult.StatusCode = httpResponse.StatusCode;

            return responseResult;
        }

        public static Result<TEntity> GetResult<TEntity>(HTTPResponse httpResponse) where TEntity : class 
        {
            Result<TEntity> responseResult = new Result<TEntity>();

            if (httpResponse == null)
            {
                responseResult.IsSuccess = false;
                responseResult.Message = "Unknown request error occurred";
                responseResult.StatusCode = 0;

                return responseResult;
            }

            if (httpResponse.IsSuccess)
            {
                responseResult.IsSuccess = true;
                responseResult.Entity = RetrieveData<TEntity>(httpResponse.Data);
            }
            else
            {
                responseResult.IsSuccess = false;
            }

            responseResult.Aborted = httpResponse.RequestAborted;
            responseResult.Message = httpResponse.Error;
            responseResult.StatusCode = httpResponse.StatusCode;

            return responseResult;
        }

        private static TEntity RetrieveData<TEntity>(byte[] data) where TEntity : class
        {
            if (data == null)
            {
                return default;
            }

            if (typeof(TEntity) == typeof(byte[]))
            {
                return data as TEntity;
            }
            else
            {
                string json = DecodeJson(data);
                TEntity entity = DeserialzieJson<TEntity>(json);
                return entity;
            }
        }

        private static string DecodeJson(byte[] data)
        {
            return System.Text.Encoding.UTF8.GetString(data);
        }

        private static TResult DeserialzieJson<TResult>(string json)
        {
            if (json == null)
            {
                return default;
            }

            try
            {
                TResult response = JsonConvert.DeserializeObject<TResult>(json);
                return response;
            }
            catch (Exception exp)
            {
                Debug.LogError($"Error happend during http response deserialization: \"{exp.Message}\"");
            }

            return default;
        }
    }
}
