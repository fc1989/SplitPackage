﻿using SplitPackage.Split.SplitModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SplitPackage.Models
{
    public abstract class BaseRequest
    {
        /// <summary>
        /// 认证信息（用户名）
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 货品列表
        /// </summary>
        public List<Product> ProList { get; set; }
        /// <summary>
        /// 该订单商品总数量
        /// </summary>
        public int TotalQuantity { get; set; }
    }
}