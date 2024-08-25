using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;

namespace MVC_FinalTerm.Repository.Sessions
{
    public static class SessionExtensions
    {
        public static void SetJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        public static T GetJson<T>(this ISession session, string key)
        {
            var sessionData = session.GetString(key);
            if (sessionData == null)
            {
                return default(T);
            }

            // Kiểm tra xem dữ liệu là đối tượng hay mảng JSON
            if (sessionData.StartsWith("{") && sessionData.EndsWith("}"))
            {
                // Dữ liệu là đối tượng JSON
                return JsonConvert.DeserializeObject<T>(sessionData);
            }
            else if (sessionData.StartsWith("[") && sessionData.EndsWith("]"))
            {
                // Dữ liệu là mảng JSON
                // Giả sử T là một kiểu List<T>
                return JsonConvert.DeserializeObject<T>(sessionData, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });
            }
            else
            {
                return default(T);
            }
        }
    }

    public static class CookieExtensions
    {
        public static void SetJson(this IResponseCookies cookies, string key, object value, int? expireTime)
        {
            var option = new CookieOptions();
            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
            else
                option.Expires = DateTime.Now.AddMinutes(60);

            cookies.Append(key, JsonConvert.SerializeObject(value), option);
        }

        public static T GetJson<T>(this IRequestCookieCollection cookies, string key)
        {
            var cookieData = cookies[key];
            return cookieData == null ? default(T) : JsonConvert.DeserializeObject<T>(cookieData);
        }
    }
}
