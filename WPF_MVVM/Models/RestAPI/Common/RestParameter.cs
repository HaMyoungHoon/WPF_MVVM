using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace WPF_MVVM.Models.RestAPI.Common
{
    internal class RestParameter
    {
        readonly IList<string> _path;
        readonly IDictionary<string, object> _params;
        string? _classParams;
        readonly IDictionary<string, object> _headers;
        readonly IDictionary<string, object?> _forms;
        object? _multipartForm;
        public RestParameter()
        {
            _path = new List<string>();
            _params = new Dictionary<string, object>();
            _headers = new Dictionary<string, object>();
            _forms = new Dictionary<string, object?>();
            _multipartForm = null;
        }

        public string GetPaths()
        {
            return string.Join("/", _path);
        }
        public string GetParams()
        {
            StringBuilder stb = new();
            foreach (var key in _params.Keys)
            {
                stb.Append($"{key}={_params[key]}&");
            }
            if (stb.Length > 0)
            {
                if (stb[^1] == '&')
                {
                    stb.Remove(stb.Length - 1, 1);
                }
            }
            if (_classParams != null && _classParams.Length > 0)
            {
                if (stb.Length > 0)
                {
                    stb.Append($"&{_classParams}");
                }
                else
                {
                    stb.Append(_classParams);
                }
            }
            return stb.ToString();
        }
        public IList<string> GetHeaders()
        {
            IList<string> ret = new List<string>();
            foreach (var key in _headers.Keys)
            {
                ret.Add($"{key}: {_headers[key]}");
            }
            return ret;
        }
        public IDictionary<string, object> GetHeaders2()
        {
            return _headers;
        }
        public string GetForms()
        {
            if (_forms.Count <= 0)
            {
                throw new ArgumentNullException();
            }
            var buff = new JsonArray()
            {
                _forms
            };
            return buff[0]?.ToJsonString().Replace("\"[", "[").Replace("]\"", "]").Replace(@"\u0022", "\"") ?? string.Empty;
        }
        public MultipartFormDataContent? GetMultipartForms()
        {
            if (_multipartForm == null)
            {
                throw new ArgumentNullException();
            }

            var ret = new MultipartFormDataContent();

            var list = (from p in _multipartForm?.GetType().GetProperties()
                        where p.GetValue(_multipartForm, null) != null
                        select (p.Name.ToString(), p.GetValue(_multipartForm)?.ToString() ?? null));
            if (list == null)
            {
                throw new ArgumentNullException();
            }

            foreach (var data in list)
            {
                var asdf1 = data.Item1;
                var asdf2 = data;

                if (data.Item2 == null)
                {
                    continue;
                }

                if (data.Item2.Contains("[FILE]:"))
                {
                    var file = File.OpenRead(data.Item2.Replace("[FILE]:", string.Empty));
                    var fileName = file.Name[(file.Name.LastIndexOf("\\") + 1)..];
                    ret.Add(new StreamContent(file), data.Item1, fileName);
                }
                else
                {
                    ret.Add(new StringContent(data.Item2), data.Item1);
                }
            }
            return ret;
        }
        public int GetPathsCount()
        {
            return _path.Count;
        }
        public int GetParamsCount()
        {
            return _params.Count + (_classParams?.Length ?? 0);
        }
        public int GetHeadersCount()
        {
            return _headers.Count;
        }
        public int GetFormsCount()
        {
            return _forms?.Count ?? 0;
        }
        public int GetMultipartFormCount()
        {
            return _multipartForm != null ? 1 : 0;
        }
        public void AddPath(string value)
        {
            _path.Add(value);
        }
        public void AddParameter(string key, object value)
        {
            if (!_params.ContainsKey(key))
            {
                string valueBuff;
                if (value.GetType().Name.ToLower() == "class")
                {
                    valueBuff = JsonSerializer.Serialize(value);
                }
                else if (value.GetType() == typeof(IList<string>) || value.GetType() == typeof(List<string>))
                {
                    valueBuff = string.Join(",", value as List<string> ?? new List<string>());
                }
                else
                {
                    valueBuff = value.ToString() ?? string.Empty;
                }
                _params.Add(key, valueBuff);
            }
        }
        public void AddParameter(object value)
        {
            _classParams = string.Join("&", from p in value.GetType().GetProperties()
                                            where p.GetValue(value, null) != null
                                            select $"{string.Concat(p.Name[0].ToString().ToLower(), p.Name.AsSpan(1))}={(p.PropertyType.Name == typeof(IList<>).Name ? string.Join(",", p.GetValue(value, null) as List<string> ?? new List<string>()) : p.GetValue(value, null)?.ToString())}");
        }
        public void AddHeaders(string key, object value)
        {
            if (!_headers.ContainsKey(key))
            {
                string valueBuff;
                if (value.GetType().Name.ToLower() == "class")
                {
                    valueBuff = JsonSerializer.Serialize(value);
                }
                else
                {
                    valueBuff = value.ToString() ?? string.Empty;
                }
                _headers.Add(key, valueBuff);
            }
        }
        public void AddBody(string key, object value)
        {
            if (value.GetType() == typeof(IList<string>) || value.GetType() == typeof(List<string>))
            {
                _forms.Add(key, JsonSerializer.Serialize(value));
            }
            else
            {
                string valueBuff = value.ToString() ?? string.Empty;
                _forms.Add(key, valueBuff);
            }
        }
        public void SetMultipartForms(object obj)
        {
            _multipartForm = obj;
            //            var buff = from p in obj.GetType().GetProperties()
            //                       where p.GetValue(obj, null)?.ToString()?.ToUpper().Contains("[FILE]:") == true
            //                       select p.Name;
            //            if (buff?.Count() <= 0)
            //            {
            //                MergeBody(obj);
            //            }
            //            else
            //            {
            //                _multipartForm = obj;
            //            }
        }
        public void MergeBody(object? data)
        {
            if (data == null)
            {
                return;
            }

            _forms.Clear();
            JsonObject? jobject = JsonNode.Parse(JsonSerializer.Serialize(data))?.AsObject();
            if (jobject == null)
            {
                return;
            }

            foreach (var item in jobject)
            {
                if (item.Value == null)
                {
                    continue;
                }
                _forms.Add(item.Key, item.Value);
            }
        }
    }
}
