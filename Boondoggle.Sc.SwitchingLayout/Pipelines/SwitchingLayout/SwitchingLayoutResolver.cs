using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Web;

namespace Boondoggle.Sc.Pipelines.SwitchingLayout
{
    public class SwitchingLayoutResolver : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (Sitecore.Context.Item == null)
                return;

            var layoutItem = GetQueryStringLayout() ?? Sitecore.Context.Item.Visualization.Layout;
            if (layoutItem != null && layoutItem.InnerItem.TemplateID.ToString() == "{2C87C6FE-F615-4E92-B321-1BAE3D488B0D}")
            {
                Tracer.Info("Found SwitchingLayout");

                ReferenceField layoutField = null;
                if (Sitecore.Context.PageMode.IsExperienceEditor)
                {
                    layoutField = (ReferenceField)layoutItem.InnerItem.Fields["EditingLayout"];
                }
                else
                {
                    layoutField = (ReferenceField)layoutItem.InnerItem.Fields["NormalLayout"];
                }

                if (layoutField != null)
                {
                    var specificLayoutItem = layoutField.TargetItem;
                    if (specificLayoutItem != null)
                    {
                        Tracer.Info(string.Format("[{0}] Setting SwitchingLayout to {1}", Sitecore.Context.PageMode.IsExperienceEditor ? "Editing" : "Normal", specificLayoutItem.ID.ToString()));

                        var newLayout = new LayoutItem(specificLayoutItem);
                        Sitecore.Context.Page.FilePath = newLayout.FilePath;
                        args.Context.Items.Add("SwitchingLayoutGuid", specificLayoutItem.ID.ToGuid());
                    }
                }
            }
        }

        /// <summary>
        /// Gets the layout.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// The layout.
        /// </returns>
        private static LayoutItem GetQueryStringLayout()
        {
            string queryString = WebUtil.GetQueryString("sc_layout");
            if (string.IsNullOrEmpty(queryString))
                return null;

            return Sitecore.Context.Database.GetItem(queryString);
        }
    }
}
