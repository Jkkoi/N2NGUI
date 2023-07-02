using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace N2NGUI;

public class DataValidating : IDataErrorInfo
{
    [Regex("(((\\d{1,2})|(1\\d{2})|(2[0-4]\\d)|(25[0-5]))\\.){3}((\\d{1,2})|(1\\d{2})|(2[0-4]\\d)|(25[0-5]))")]
    public string? LanAddress { get; set; }

    [Regex("^(\\d|[1-9]\\d|1\\d{2}|2[0-4]\\d|25[0-5])\\.(\\d|[1-9]\\d|1\\d{2}|2[0-4]\\d|25[0-5])\\.(\\d|[1-9]\\d|1\\d{2}|2[0-4]\\d|25[0-5])\\.(\\d|[1-9]\\d|1\\d{2}|2[0-4]\\d|25[0-5]):([0-9]|[1-9]\\d|[1-9]\\d{2}|[1-9]\\d{3}|[1-5]\\d{4}|6[0-4]\\d{3}|65[0-4]\\d{2}|655[0-2]\\d|6553[0-5])$")]
    public string? ServerAddress { get; set; }

    [Regex("^[a-zA-Z\\d]+$")]
    public string? Community { get; set; }
    [Regex("^[a-zA-Z\\d]+$")]
    public string? Password { get; set; }
    public string Error => String.Empty;
    
    public string this[string columnName]
    {
        get
        {
            PropertyInfo? pInfo = this.GetType().GetProperty(columnName, BindingFlags.Public | BindingFlags.Instance);


            if (pInfo != null && pInfo.IsDefined(typeof(RegexAttribute), false))
            {
                RegexAttribute? attr = (RegexAttribute?)pInfo.GetCustomAttribute(typeof(RegexAttribute), false);
                //如果属性没有应用该特性
                if (attr == null) { return string.Empty; }

                if (!Regex.IsMatch(pInfo.GetValue(this)?.ToString() ?? string.Empty, attr.PattenValue))
                {
                    return "格式有误";
                }
                //如果属性值为空
                // else if(pInfo.GetValue(this) == null) { return string.Empty; }
                // //如果属性值为定义的ValidateValue
                // else if (pInfo.GetValue(this).ToString() == attr.PattenPatten)
                // {
                //     return "字段不能为" + attr.PattenPatten;
                // }
            }
            return string.Empty;
        }
    }

}

public class RegexAttribute : Attribute
{
    public string PattenValue { get; set; }

    public RegexAttribute(string patten)
    {
        PattenValue = patten;
    }
}