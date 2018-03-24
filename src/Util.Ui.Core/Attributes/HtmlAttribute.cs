﻿using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Util.Helpers;
using Util.Logs;
using Util.Logs.Extensions;

namespace Util.Ui.Attributes {
    /// <summary>
    /// 生成Html静态文件
    /// </summary>
    public class HtmlAttribute : ActionFilterAttribute {
        /// <summary>
        /// 是否忽略，设置为true则不生成html文件
        /// </summary>
        public bool Ignore { get; set; }

        /// <summary>
        /// 生成路径，相对根路径，范例：/Typings/app/app.component.html
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 执行生成
        /// </summary>
        public override async Task OnResultExecutionAsync( ResultExecutingContext context, ResultExecutionDelegate next ) {
            await WriteViewToFileAsync( context );
            await base.OnResultExecutionAsync( context, next );
        }

        /// <summary>
        /// 将视图写入html文件
        /// </summary>
        private async Task WriteViewToFileAsync( ResultExecutingContext context ) {
            try {
                if( Ignore )
                    return;
                var html = await RenderToStringAsync( context );
                if( string.IsNullOrWhiteSpace( html ) )
                    return;
                var path = Util.Helpers.Common.GetPhysicalPath( string.IsNullOrWhiteSpace( Path ) ? GetPath( context ) : Path );
                File.WriteAllText( path, html );
            }
            catch( Exception ex ) {
                ex.Log( Log.GetLog().Caption( "生成html静态文件失败" ) );
            }
        }

        /// <summary>
        /// 渲染视图
        /// </summary>
        public async Task<string> RenderToStringAsync( ResultExecutingContext context ) {
            string viewName = "";
            object model = null;
            if ( context.Result is ViewResult result ) {
                viewName = string.IsNullOrWhiteSpace( viewName ) ? context.RouteData.Values["action"].SafeString() : viewName;
                model = result.Model;
            }
            var razorViewEngine = Ioc.Create<IRazorViewEngine>();
            var tempDataProvider = Ioc.Create<ITempDataProvider>();
            var serviceProvider = Ioc.Create<IServiceProvider>();
            var httpContext = new DefaultHttpContext { RequestServices = serviceProvider };
            var actionContext = new ActionContext( httpContext, context.RouteData, new ActionDescriptor() );
            using( var stringWriter = new StringWriter() ) {
                var viewResult = razorViewEngine.FindView( actionContext, viewName, true );
                if( viewResult.View == null )
                    throw new ArgumentNullException( $"未找到视图： {viewName}" );
                var viewDictionary = new ViewDataDictionary( new EmptyModelMetadataProvider(), new ModelStateDictionary() ) { Model = model };
                var viewContext = new ViewContext( actionContext, viewResult.View, viewDictionary, new TempDataDictionary( actionContext.HttpContext, tempDataProvider ), stringWriter, new HtmlHelperOptions() );
                await viewResult.View.RenderAsync( viewContext );
                return stringWriter.ToString();
            }
        }

        /// <summary>
        /// 获取Html默认生成路径
        /// </summary>
        protected virtual string GetPath( ResultExecutingContext context ) {
            var area = context.RouteData.Values["area"];
            var controller = context.RouteData.Values["controller"];
            var action = context.RouteData.Values["action"];
            if( string.IsNullOrWhiteSpace( area.SafeString() ) )
                return $"Typings/app/{controller}/{action}.component.html";
            return $"Typings/app/{area}/{controller}/{controller}-{action}.component.html";
        }
    }
}
