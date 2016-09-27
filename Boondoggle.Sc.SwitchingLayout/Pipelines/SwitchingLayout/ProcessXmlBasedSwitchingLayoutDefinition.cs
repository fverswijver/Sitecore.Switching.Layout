using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Pipelines.Response.BuildPageDefinition;
using Sitecore.Mvc.Presentation;

namespace Boondoggle.Sc.Pipelines.SwitchingLayout
{
    public class ProcessXmlBasedSwitchingLayoutDefinition : ProcessXmlBasedLayoutDefinition
    {
        protected override Rendering GetRendering(XElement renderingNode, Guid deviceId, Guid layoutId, string renderingType, XmlBasedRenderingParser parser)
        {
            var rendering = base.GetRendering(renderingNode, deviceId, layoutId, renderingType, parser);
            try
            {
                if (!String.IsNullOrWhiteSpace(renderingType) && renderingType == "Layout")
                {
                    if (HttpContext.Current.Items.Contains("SwitchingLayoutGuid"))
                    {
                        var switchingLayoutId = (Guid)HttpContext.Current.Items["SwitchingLayoutGuid"];

                        Tracer.Info("Setting XmlBasedLayoutDefinition SwitchingLayout to " + switchingLayoutId.ToString());
                        rendering.LayoutId = switchingLayoutId;
                    }
                }
            }
            catch (Exception ex)
            {
                Tracer.Error("Unable to process XmlBasedLayoutDefinition SwitchingLayout");
            }

            return rendering;
        }
    }
}
