﻿using Microsoft.Extensions.DependencyInjection;
using Util.Events.Handlers;

namespace Util.Events.Default {
    /// <summary>
    /// 事件总线扩展
    /// </summary>
    public static class Extensions {
        /// <summary>
        /// 注册事件总线服务
        /// </summary>
        /// <param name="services">服务集合</param>
        public static void AddEventBus( this IServiceCollection services ) {
            services.AddSingleton<IEventHandlerManager, EventHandlerManager>();
            services.AddSingleton<IEventBus, Util.Events.Default.EventBus>();
        }
    }
}
