﻿using Microsoft.AspNetCore.Razor.TagHelpers;
using Util.Ui.Configs;
using Util.Ui.Material.Buttons.Renders;
using Util.Ui.Material.Enums;
using Util.Ui.Renders;
using Util.Ui.TagHelpers;

namespace Util.Ui.Material.Buttons.TagHelpers {
    /// <summary>
    /// 链接
    /// </summary>
    [HtmlTargetElement("util-a")]
    public class ATagHelper : TagHelperBase {
        /// <summary>
        /// 标识，指向模板引用变量，而不是Id属性
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 文本
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 样式
        /// </summary>
        public ButtonStyle Styles { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public Color Color { get; set; }
        /// <summary>
        /// 禁用
        /// </summary>
        public string Disabled { get; set; }
        /// <summary>
        /// 提示
        /// </summary>
        public string Tooltip { get; set; }
        /// <summary>
        /// 路由链接地址
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// 单击事件处理函数,范例：handle()
        /// </summary>
        public string OnClick { get; set; }

        /// <summary>
        /// 获取渲染器
        /// </summary>
        /// <param name="context">上下文</param>
        protected override IRender GetRender( Context context ) {
            return new AnchorRender( new Config( context ) );
        }
    }
}
