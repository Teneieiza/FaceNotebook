using FaceNoteBook.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FaceNoteBook.Utils
{
    public static class ApiResponseHelper
    {
        public static ActionResult<ApiResponse<T>> Success<T>(T data, int statusCode = 200, string message = "OK")
        {
            return new ObjectResult(new ApiResponse<T>(statusCode, message, data))
            {
                StatusCode = statusCode
            };
        }
    }
}