using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Diagnostics.Metrics;
using System.Text;

namespace QCard.ManagementSystem.Web.Helper
{
    public static class HtmlHelperExtension
    {
        /// <summary>
        /// Containing form tag must have "form-horizontal" class in order for this to be working.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="label"></param>
        /// <param name="labelColumn">Number of column assigned based on bootstrap col-{-{size}</param>
        /// <param name="maxLength"></param>
        /// <param name="isRequired"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IHtmlContent InlineTextbox(this IHtmlHelper helper, string id, string label, string labelClass, string fieldClass, int maxLength, bool isRequired, string value = "", bool isReadOnly = false)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<div class=\"form-group\">");
            sb.AppendLine($"    <label class=\"control-label {labelClass}\" style=\"text-align:left\">{label}");
            if (isRequired)
            {
                sb.Append("<span class=\"required\"> *</span></label>");
            }
            else
            {
                sb.Append("</label>");
            }
            sb.AppendLine($"    <div class=\"{fieldClass}\">");
            sb.AppendLine($"        <input class=\"form-control\" type=\"text\" id=\"{id}\" maxlength=\"{maxLength}\" name=\"{id}\" value=\"{value}\" {(isReadOnly ? "readonly" : null)}>");
            sb.AppendLine($"        <label id=\"lbl{id}Error\" style=\"display:none\" class=\"required\" />");
            sb.AppendLine($"    </div>");
            sb.AppendLine($"</div>");

            return new HtmlString(sb.ToString());
        }
        public static IHtmlContent InlineTextboxReadonly(this IHtmlHelper helper, string id, string label, string labelClass, string fieldClass, int maxLength, bool isRequired, string value = "")
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<div class=\"form-group\">");
            sb.AppendLine($"    <label class=\"control-label {labelClass}\" style=\"text-align:left\">{label}");
            if (isRequired)
            {
                sb.Append("<span class=\"required\"> *</span></label>");
            }
            else
            {
                sb.Append("</label>");
            }
            sb.AppendLine($"    <div class=\"{fieldClass}\">");
            sb.AppendLine($"        <input class=\"form-control\" type=\"text\" id=\"{id}\" maxlength=\"{maxLength}\" name=\"{id}\" value=\"{value}\" readonly>");
            sb.AppendLine($"        <label id=\"lbl{id}Error\" style=\"display:none\" class=\"required\" />");
            sb.AppendLine($"    </div>");
            sb.AppendLine($"</div>");

            return new HtmlString(sb.ToString());
        }

        public static IHtmlContent GeneralDropdown(this IHtmlHelper helper, string id, string label, string labelClass, string fieldClass, string dataUrl, string requiredMessage = "")
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<div class=\"form-group row\">");
            sb.AppendLine($"    <label class=\"control-label {labelClass}\" style=\"text-align:left\">{label}</label>");
            sb.AppendLine($"    <div class=\"{fieldClass}\">");
            sb.AppendLine($"        <input type=\"text\" id=\"{id}\" name=\"{id}\" class=\"form-control\" style=\"width: 100%;\" required=\"required\" />");
            sb.AppendLine($"    </div>");
            sb.AppendLine("</div>");

            sb.AppendLine("<script type=\"text/javascript\">");
            sb.AppendLine("$(\"#" +id+ "\").kendoDropDownList({");
            sb.AppendLine("dataTextField: \"text\",");
            sb.AppendLine("    dataValueField: \"value\",");
            sb.AppendLine("    dataSource:");
            sb.AppendLine("    {");
            sb.AppendLine("    transport:");
            sb.AppendLine("        {");
            sb.AppendLine("        read:");
            sb.AppendLine("            {");
            sb.AppendLine("            cache: false,");
            sb.AppendLine("                url: \"" + dataUrl + "\",");
            sb.AppendLine("                dataType: \"json\"");
            sb.AppendLine("            },");
            sb.AppendLine("        },");
            sb.AppendLine("    },");
            sb.AppendLine("    index: 0,");
            sb.AppendLine("    filter: \"contains\",");
            sb.AppendLine("    suggest: true,");
            sb.AppendLine("    optionLabel:");
            sb.AppendLine("    {");
            sb.AppendLine("    text: \"ALL\",");
            sb.AppendLine("        value: \"\"");
            sb.AppendLine("    },");
            sb.AppendLine("    popup:");
            sb.AppendLine("    {");
            sb.AppendLine("    appendTo: $(\"#bodyModal\")");
            sb.AppendLine("    },");
            sb.AppendLine("});");
            sb.AppendLine("</script>");

            return new HtmlString(sb.ToString());
        }
    }
}
