using System;
using System.Text.Json.Serialization;

namespace KvsProject.Domain
{
    public class Result
    {
        public List<string?> Errors { get; set; }
        public bool HasError => Errors.Any();
        [JsonIgnore] public Dictionary<string, object> Extra { get; set; }

        public string GetErrorMessage(string separator = "\r\n")
        {
            return string.Join(separator, Errors);
        }

        public Result()
        {
            Errors = new List<string?>();
            Extra = new Dictionary<string, object>();
        }

        public Result(string errorMessage)
        {
            Errors = new List<string?>()
            {
                errorMessage
            };
            Extra = new Dictionary<string, object>();
        }

        public Result(EntityValidationResult validationResult)
        {
            if (validationResult.ValidationErrors != null)
            {
                Errors = validationResult.ValidationErrors.Select(a => a.ErrorMessage).Distinct().ToList();
            }
            else
            {
                Errors = new List<string?>();
            }
            Extra = new Dictionary<string, object>();
        }

        public Result(Result other)
        {
            Errors = other.Errors;
            Extra = new Dictionary<string, object>();
        }

        public T? Value<T>(string key)
        {
            if (Extra != null && Extra.ContainsKey(key))
            {
                return (T)Extra[key];
            }
            return default;
        }
        public void Set<T>(string key, T data)
        {
            if (Extra != null && data != null)
            {
                Extra[key] = data;
            }
        }
    }

    public class Result<T> : Result
    {
        public T? Data { get; set; }

        public Result() : base()
        {
        }

        public Result(string errorMessage) : base(errorMessage)
        {
        }

        public Result(EntityValidationResult validationResult) : base(validationResult)
        {
        }

        public Result(Result other) : base(other)
        {
        }
    }
}

