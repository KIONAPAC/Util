﻿using System;
using System.Threading.Tasks;
using Util.Biz.Payments;
using Util.Biz.Payments.Alipay.Results;
using Util.Biz.Payments.Alipay.Services;
using Util.Biz.Payments.Core;
using Util.Biz.Tests.Integration.Payments.Alipay.Configs;
using Util.Exceptions;
using Util.Helpers;
using Util.Tests.XUnitHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Util.Biz.Tests.Integration.Payments.Alipay {
    /// <summary>
    /// 支付宝条码支付服务测试
    /// </summary>
    public class AlipayBarcodePayServiceTest : IDisposable {
        /// <summary>
        /// 控制台输出
        /// </summary>
        private readonly ITestOutputHelper _output;
        /// <summary>
        /// 支付宝条码支付服务
        /// </summary>
        private AlipayBarcodePayService _service;

        /// <summary>
        /// 测试初始化
        /// </summary>
        public AlipayBarcodePayServiceTest( ITestOutputHelper output ) {
            _output = output;
            _service = new AlipayBarcodePayService( new TestConfigProvider() );
        }

        /// <summary>
        /// 测试清理
        /// </summary>
        public void Dispose() {
            Time.Reset();
        }

        /// <summary>
        /// 验证订单标题为空时，将订单号赋值给订单标题
        /// </summary>
        [Fact]
        public void TestPay_Validate_Subject() {
            var param = new PayParam {
                Money = 1,
                OrderId = "a",
                AuthCode = "1"
            };
            _service.Pay( param );
            Assert.Equal( "a", param.Subject );
        }

        /// <summary>
        /// 验证订单号为空
        /// </summary>
        [Fact]
        public void TestPay_Validate_OrderId() {
            AssertHelper.Throws<Warning>( () => {
                _service.Pay( new PayParam {
                    Money = 1,
                    AuthCode = "1",
                    Subject = "a"
                } );
            }, PayResource.OrderIdIsEmpty );
        }

        /// <summary>
        /// 验证无效金额
        /// </summary>
        [Fact]
        public void TestPay_Validate_Money() {
            AssertHelper.Throws<Warning>( () => {
                _service.Pay( new PayParam {
                    Money = 0,
                    OrderId = "a",
                    Subject = "a",
                    AuthCode = "1"
                } );
            }, PayResource.InvalidMoney );
        }

        /// <summary>
        /// 验证用户付款授权码为空
        /// </summary>
        [Fact]
        public void TestPay_Validate_AuthCode() {
            AssertHelper.Throws<Warning>( () => {
                _service.Pay( new PayParam {
                    Money = 10,
                    OrderId = "a",
                    Subject = "test"
                } );
            }, PayResource.AuthCodeIsEmpty );
        }

        /// <summary>
        /// 测试支付
        /// </summary>
        [Fact]
        public void TestPay_1() {
            var result = "app_id=2016090800463464&biz_content={\"out_trade_no\":\"59f7caeeab89e009e4a4e1fb\",\"subject\":\"test\",\"total_amount\":\"10\",\"timeout_express\":\"90m\",\"auth_code\":\"281023564031402341\",\"scene\":\"bar_code\"}&charset=utf-8&format=json&method=alipay.trade.pay&sign=TrzaFo0mYi5ECKxQqwVj8WeNivimDYdeZfRF9bGV4Hq2rAoaZLshvV6C/1oeGJq/jsfiFFvxi6RNSlJuo2+erZq81FYALKltt+8gL6XwgCf8KL64+nC3zpE1dAmkKJA3ft4xKoEG5uUSMQBKqx59E3DNApAeibrFboF5vP1MB/Dru1pfS7mijixWhPd1LSDMdH0tUCyWlkh1W1MiWnrzBCNNEaNn2slHvYQjyUHZyR577yuCdcst5/MjCwY+0l0Rt0QYaCPizQEU1n6h65gifq/sSyOlkLnaMeX2JZpRSD7yCVzSyMHqsYRAr5vzSBztfYIrMVdb74JkQQKa27QVng==&sign_type=RSA2&timestamp=2017-10-31 08:59:26&version=1.0";
            Time.SetTime( "2017-10-31 08:59:26" );
            _service = new AlipayBarcodePayService( new TestConfigProvider() ) { IsSendRequest = false };
            _service.Pay( new PayParam {
                Money = 10,
                OrderId = "59f7caeeab89e009e4a4e1fb",
                Subject = "test",
                AuthCode = "281023564031402341"
            } );
            _output.WriteLine( _service.ToString() );
            Assert.Equal( result, _service.ToString() );
        }

        /// <summary>
        /// 测试支付
        /// </summary>
        [Fact( Skip = "请下载沙箱支付宝，填写正确的付款码" )]
        public void TestPay_2() {
            var result = _service.Pay( new PayParam {
                Money = 10,
                OrderId = Id.Guid(),
                AuthCode = "280299913207329986"
            } );
            _output.WriteLine( result.Message );
            Assert.True( result.Success );
        }

        /// <summary>
        /// 测试支付
        /// </summary>
        [Fact( Skip = "请下载沙箱支付宝，填写正确的付款码" )]
        public async Task TestPayAsync() {
            var result = await _service.PayAsync( new PayParam {
                Money = 10,
                OrderId = Id.Guid(),
                AuthCode = "281296574204429642"
            } );
            _output.WriteLine( result.Message );
            Assert.True( result.Success );
        }

        /// <summary>
        /// 测试Json转换
        /// </summary>
        [Fact]
        public void TestJson() {
            const string json = "{\"alipay_trade_pay_response\":{\"code\":\"10000\",\"msg\":\"Success\",\"buyer_logon_id\":\"jeu*** @sandbox.com\",\"buyer_pay_amount\":\"10.00\",\"buyer_user_id\":\"2088102174804335\",\"fund_bill_list\":[{\"amount\":\"10.00\",\"fund_channel\":\"ALIPAYACCOUNT\"}],\"gmt_payment\":\"2017-10-31 13:23:12\",\"invoice_amount\":\"10.00\",\"open_id\":\"20881058260191225496241750118233\",\"out_trade_no\":\"06bfa9807832438dac6a2b785ad7addc\",\"point_amount\":\"0.00\",\"receipt_amount\":\"10.00\",\"total_amount\":\"10.00\",\"trade_no\":\"2017103121001004330200344580\"},\"sign\":\"CkMKbLdFmlhIz0Ymob7IjnGdjBDfAt5/aAZ7l0jMvwFJyBRf0TaRJiHfXTCI7srL68RQ5DnR6N89XSr1+MiclVbpbNa3joi4XDd1sdEkTKMcEmp28tvL9q3UAbMtwKgiS93CWjmj/D5xK7K+ZxwVPwF3JlkeCd2Qg5GAtHmNjAAt3tlEKVn+SmRQ0yKyk2PpvVSSBBYbFo+VircmOxHo/m/ji3sK68y0ikhQYhHRuNQXXTp3KellpIESaIUGHi8KdQa7lV2acnDnSDChWy/4PxIrEmm8Ki8PMKsqS8WiIwKiUTldeWGZ0D749oP4iq6n18iDtjmSDeEgOTkhErVKLg==\"}";
            var result = new AlipayResult( json );
            Assert.Equal( "10000", result.GetCode() );
            Assert.Equal( "2017103121001004330200344580", result.GetTradeNo() );
            Assert.True( result.HasKey( "sign" ) );
        }
    }
}
