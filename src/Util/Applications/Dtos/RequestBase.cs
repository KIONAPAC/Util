﻿using System.Linq;
using System.Runtime.Serialization;
using Util.Exceptions;
using Util.Validations;

namespace Util.Applications.Dtos {
    /// <summary>
    /// 请求对象
    /// </summary>
    [DataContract]
    public abstract class RequestBase : IRequest {
        /// <summary>
        /// 验证
        /// </summary>
        public virtual ValidationResultCollection Validate() {
            var result = DataAnnotationValidation.Validate( this );
            if( result.IsValid )
                return ValidationResultCollection.Success;
            throw new Warning( result.First().ErrorMessage );
        }
    }
}
