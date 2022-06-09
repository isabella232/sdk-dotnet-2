// These tests are not valid on net462 because we do not convert pem to x509certificate2
#if NETCOREAPP3_1_OR_GREATER
using System;
using System.Security.Cryptography.X509Certificates;
using FluentAssertions;
using Temporal.WorkflowClient;
using Xunit;

namespace Temporal.Sdk.WorkflowClient.Test.Int
{
    [Trait("TestCategory", "Unit")]
    public class X509CertTest
    {
        private const string ECDSA128Certificate = "LS0tLS1CRUdJTiBDRVJUSUZJQ0FURS0tLS0tCk1JSEtNSUdSQWdrQTVOeEVuTGVvMWFzd0NnWUlLb1pJemowRUF3SXdEekVOTUFzR0ExVUVBd3dFVkdWemREQWUKRncweU1qQTJNRFl5TXpJME1qWmFGdzB6TWpBME1UUXlNekkwTWpaYU1BOHhEVEFMQmdOVkJBTU1CRlJsYzNRdwpOakFRQmdjcWhrak9QUUlCQmdVcmdRUUFIQU1pQUFTMURRZ2o2akM1M2hYREs0bGdUbU5JaFBGQTdNL2hhV0lmCkNIbTRoZ3JaM1RBS0JnZ3Foa2pPUFFRREFnTW9BREFsQWhBRTZHWHR1ZVBMYUttVDBLUVk3cXQzQWhFQTh3NmoKcGZaRStXeHJ1OW15aHBPZk5BPT0KLS0tLS1FTkQgQ0VSVElGSUNBVEUtLS0tLQo=";
        //private const string ECDSA128Key = "LS0tLS1CRUdJTiBFQyBQUklWQVRFIEtFWS0tLS0tCk1FUUNBUUVFRU9KKy9RZjBrc2JabWxERnh3a0JFWHFnQndZRks0RUVBQnloSkFNaUFBUzFEUWdqNmpDNTNoWEQKSzRsZ1RtTkloUEZBN00vaGFXSWZDSG00aGdyWjNRPT0KLS0tLS1FTkQgRUMgUFJJVkFURSBLRVktLS0tLQo=";
        private const string ECDSA256Certificate = "LS0tLS1CRUdJTiBDRVJUSUZJQ0FURS0tLS0tCk1JSUJERENCdEFJSkFMYU53RkcyNHN6OE1Bb0dDQ3FHU000OUJBTUNNQTh4RFRBTEJnTlZCQU1NQkZSbGMzUXcKSGhjTk1qSXdOakEyTWpNeU5ESTJXaGNOTXpJd05ERTBNak15TkRJMldqQVBNUTB3Q3dZRFZRUUREQVJVWlhOMApNRmt3RXdZSEtvWkl6ajBDQVFZSUtvWkl6ajBEQVFjRFFnQUV5L2JVT2phSW05d1F3TjYyeWVib3c2OGgwbnB4ClJUMjBUZEE3dXQ5TmdHOHdMeW1HR2Q0NTBmNHlEdk0vTGpFbU4xTGF6QngyelBUNTRvVjRhelZSdGpBS0JnZ3EKaGtqT1BRUURBZ05IQURCRUFpQU1TRW1OODlyZXp0dUx0d2Z6UndmRnpiUVg1ZDZ4VksvWXZiOEFVeDNlUEFJZwpkdllLYjgvS2gxbE80Y3ZaTG9JRWVCaXE1OHZPRmdpbEk4Szc4T0I1ZGY4PQotLS0tLUVORCBDRVJUSUZJQ0FURS0tLS0tCg==";
        private const string ECDSA256Key = "LS0tLS1CRUdJTiBFQyBQUklWQVRFIEtFWS0tLS0tCk1IY0NBUUVFSUEvWTV1MnFQZzdwb3R3T3RyUC9EODl1eTYwSFA4d2cxdk04RmVlVVU5WHlvQW9HQ0NxR1NNNDkKQXdFSG9VUURRZ0FFeS9iVU9qYUltOXdRd042MnllYm93NjhoMG5weFJUMjBUZEE3dXQ5TmdHOHdMeW1HR2Q0NQowZjR5RHZNL0xqRW1OMUxhekJ4MnpQVDU0b1Y0YXpWUnRnPT0KLS0tLS1FTkQgRUMgUFJJVkFURSBLRVktLS0tLQo=";
        private const string ECDSA512Certificate = "LS0tLS1CRUdJTiBDRVJUSUZJQ0FURS0tLS0tCk1JSUJrakNCOXdJSkFNOUsyVDd3Wk5TS01Bb0dDQ3FHU000OUJBTUNNQTh4RFRBTEJnTlZCQU1NQkZSbGMzUXcKSGhjTk1qSXdOakEyTWpNeU5ESTJXaGNOTXpJd05ERTBNak15TkRJMldqQVBNUTB3Q3dZRFZRUUREQVJVWlhOMApNSUdiTUJRR0J5cUdTTTQ5QWdFR0NTc2tBd01DQ0FFQkRRT0JnZ0FFS2l2OUl6YVN5eVdGUmlHVzNTVmVZaFF5Cm16QVh1alVXK2tsU0xQZVJqY0JYclZHc1YzU29HOE9aUEs2QTY3MzNTRnMyVWo4RFIxRlp2WTB4YnltMVZLVlIKUmVTOEFYYXlSYmd0NTZYZHRMZ1Z3VC8xWFg5ZzVuUEluY1psaENRQWUwYkNrMFdWL3k3eU5kbDh1eUc5aFYrOApLMzdrb3FXWG9KazJybGJEUXhBd0NnWUlLb1pJemowRUF3SURnWWtBTUlHRkFrRUFtV0tGdEF6L29JckFhdktOCjBmRGlYMm9ZOGx5TzFwSW5FTkNiSEE1VWY4MGNZYTg2MnI1cE1UajFNWUxoSUVWVmhONkttOHRtcEZDcDMweXAKUU5NYWd3SkFkendqQkJwckRBMXhrVzJRV0lXdWU3L0JqNC9Jb2JkYlhzMjR6OURQRG1JSHp0ZlNGdmxuRGIwTQptbENnTmFhNEhuUmc3eFhFRTZCL2RSRk1zTW0vZ2c9PQotLS0tLUVORCBDRVJUSUZJQ0FURS0tLS0tCg==";
        //private const string ECDSA512Key = "LS0tLS1CRUdJTiBFQyBQUklWQVRFIEtFWS0tLS0tCk1JSGFBZ0VCQkVBY1R3WEc5Wm94cjNSdmF4V1FRdEFZQWI1ZXVEblJhNEFyUVRGUkQrbXJyeHdLOEpKZHlNWEUKZ3FUMUVKc3lsU21KL1V6aGVUVWUxd0hrNngrWjBtd1hvQXNHQ1Nza0F3TUNDQUVCRGFHQmhRT0JnZ0FFS2l2OQpJemFTeXlXRlJpR1czU1ZlWWhReW16QVh1alVXK2tsU0xQZVJqY0JYclZHc1YzU29HOE9aUEs2QTY3MzNTRnMyClVqOERSMUZadlkweGJ5bTFWS1ZSUmVTOEFYYXlSYmd0NTZYZHRMZ1Z3VC8xWFg5ZzVuUEluY1psaENRQWUwYkMKazBXVi95N3lOZGw4dXlHOWhWKzhLMzdrb3FXWG9KazJybGJEUXhBPQotLS0tLUVORCBFQyBQUklWQVRFIEtFWS0tLS0tCg==";
        private const string RSA1024Certificate = "LS0tLS1CRUdJTiBDRVJUSUZJQ0FURS0tLS0tCk1JSUJsRENCL2dJSkFQcFB6MjFLRVJlZU1BMEdDU3FHU0liM0RRRUJDd1VBTUE4eERUQUxCZ05WQkFNTUJGUmwKYzNRd0hoY05Nakl3TmpBMk1qTXlOREkyV2hjTk16SXdOakF6TWpNeU5ESTJXakFQTVEwd0N3WURWUVFEREFSVQpaWE4wTUlHZk1BMEdDU3FHU0liM0RRRUJBUVVBQTRHTkFEQ0JpUUtCZ1FENnN5ZkVQb0FnMmVCbWgrTlZxQUNuCjRzcHZlMDl5aGNTdWVyRndJendGSEp1c3Q3eVhMbzVLd0dVbW5rNDNhR0cxd0lmdFpydTlxQ0dpK1dqVWd0alcKanoxcG9EV1V1NzJmYnhodjlRSi8xQ05RTWZHTFFjTndpQUpPc2RzQjE3YjlNKzNmVkUyS0FZZEhpbVhpRnBqUgo4RmQweDJoUWw1c1Vkem8rY2lPSGl3SURBUUFCTUEwR0NTcUdTSWIzRFFFQkN3VUFBNEdCQUx0d1BwNXlRRVlqClZhdXgvaFBzUUZ1YVdqOGVhNHRNQjVRdFhtbmNjeERNL3EzQVU2Q3hJMXdwcEhydXNVR1dUVHdHc0p6ZzNNOTEKVGVsVFVKMm5lSHRCSjhCZm9UZWNBRzk0Y3BISkJzNW1nR0swWGFrUDJheUtnOUIzL0todGJYZE9LU2Y1WU9qOQpaanhoSDZCL2kyRnNXYVljOVZYTW52SnIrWXZhcXZGQwotLS0tLUVORCBDRVJUSUZJQ0FURS0tLS0tCg==";
        private const string RSA1024Key = "LS0tLS1CRUdJTiBQUklWQVRFIEtFWS0tLS0tCk1JSUNlQUlCQURBTkJna3Foa2lHOXcwQkFRRUZBQVNDQW1Jd2dnSmVBZ0VBQW9HQkFQcXpKOFErZ0NEWjRHYUgKNDFXb0FLZml5bTk3VDNLRnhLNTZzWEFqUEFVY202eTN2SmN1amtyQVpTYWVUamRvWWJYQWgrMW11NzJvSWFMNQphTlNDMk5hUFBXbWdOWlM3dlo5dkdHLzFBbi9VSTFBeDhZdEJ3M0NJQWs2eDJ3SFh0djB6N2Q5VVRZb0JoMGVLClplSVdtTkh3VjNUSGFGQ1hteFIzT2o1eUk0ZUxBZ01CQUFFQ2dZRUFoMW5vazJ4SEV2VnUwUXgyc1kxRjVWNXQKZXlqeEtOeU41c0c1UU5qVkU5RnhQeHZvQnh1TW1WRXNpMkFXekpWZmc1aFNDdHRYYXpyMkJoNXYrYVEzV3Zaagp5dG1qVEdYUWZYRnltRGZIejVjRkMrUzBTbjNzaE16cUxYS0NyYmNLM0p5dGp3Qk5NR2pDL1ptbzNlcUlHTE1PCnFaa2IwS0FxOTVraTJDSVNBWEVDUVFELzQzRk81NDlUTWdwUHZoSy9vWlFROVNCZDVTbkVtY2VMcHRGM0lxdkEKN2RyVFk1YUE2QStBUE9WdzV3MitXSnZnVy9vK3RWZXpaS0ZaSHBUODdWWkpBa0VBK3M4aU9HTE9ybUVlSmhMNgpSN1IyVFRVUmE4S1dwcVlYY093bE9VV3FWRlo5TzFRcE1lSzJsQnN2bWQxYUlGWjQwZGcxd0Z6eTI0VGZiUzlFClpDU2ZNd0pCQU11MmVDaEc5TFZZNnhpY3l6UkFJQzUzMTNLbzA5MnVSMHdJWEUycURtNGpiY2hJU2pNSmkvMmQKZlZPcXZpaEJScWNRMG1KRjRESHk5UGlML2pSMWhmRUNRUUNlSHBvZlhFT2E4a1ZVa2FCU05uWml4SE1nTWNSUwplZUhMWWtVeGJ6Y3NCbjdiT1hyakpXQWZ6V2ZjSkEzZTEwVkNYb0c4clp5NlFuR3RYeUJKMWtDN0FrQWMzSk1iCnZnNDR2THlDNkhqclhQWUo5Qmlwakd4azRNc1FuTU9DNSs0VW5OUXlnczg5cTZyaHhpd0FreWlsWFZRMFRsYW4KWThNUGdTZHg5blhtQlRDegotLS0tLUVORCBQUklWQVRFIEtFWS0tLS0tCg==";
        private const string RSA2048Certificate = "LS0tLS1CRUdJTiBDRVJUSUZJQ0FURS0tLS0tCk1JSUNtakNDQVlJQ0NRRFJ3amowM3BFVmpUQU5CZ2txaGtpRzl3MEJBUXNGQURBUE1RMHdDd1lEVlFRRERBUlUKWlhOME1CNFhEVEl5TURZd05qSXpNalF5TmxvWERUTXlNRFl3TXpJek1qUXlObG93RHpFTk1Bc0dBMVVFQXd3RQpWR1Z6ZERDQ0FTSXdEUVlKS29aSWh2Y05BUUVCQlFBRGdnRVBBRENDQVFvQ2dnRUJBTVYyc04reFhXUzNQYzVqCnU5S0R6cnQzeGEwcE9nTGs3U2syRms0d3g0Q3BYRXh4UVBKbFVXUlo3YUUwazFFQXlnd3p4VVkybWJnMWJHUlEKN3cwdnJTaUJCY2s3cThXdUo0a3BJRDZRbVFWSEhJcENPWGxXblFMZzBkZVVXZnJISjVLU0lremtISjZiUFlPWgoyOWpKOG5SblhpYVl5VXlSZ2luWVBFUVBUcVZNYW5Tc21kazY4UCtUSHhhQVpJdkh0MisyUjFXNlloa21vQ2NkCk5OVzZBMUxudmV0TjhCL28yYjhRZ0Yxdm5hQm85bDVnZlVZd1NxdHhBb05kQnlxdEI2cU1iL2pGUmczQURiM2IKTU94S1JsVE9JMUxjellLRzcvVHQ3RFdFNXA3ZXJ3WjZZQXlWbmx6YXNic0gxV2FqQW80alpuS0pJWXNpWVFhegpMVUFxK29NQ0F3RUFBVEFOQmdrcWhraUc5dzBCQVFzRkFBT0NBUUVBZGsxZjZFeVZZVzBuUU1SUjNMNktnWTR3CmRJWFRTMjlqS1ZxeDhjTElqWDNZY0lWRGliN2RkdUlpa05sbjVqcnpoN2c3MUNScnFxeEdLNmpRSzYyTys3TWUKRGMvWGF1d0YxUGN6WmxjVlBNUUwrY0djczkrY2ROY0tRQndLNUFzWnVrb1B3ZWZuL251M2NVRi9lVXVMREkybgowTFpCRlRuNDlqU0ZSNU5RVTVtNDZWNUhwWFRTYmhiSzBuMVFzSHhQeW1NT1owUDNBR2tXdVhQRkdzOFl6S0JxCmRFVnB1STVRN2cxWVU4VnVyZjVTc01wOC9hcS9wQVg0RWpnUjl4eXpBRlFqZ2s1WXpZSGxOdTV3YUgwdDJlbzUKMWxWQlU0OW44TVBvSHI3YStKQ3YxeXg2OUxHVVpnTTliRVZnNlJBU282NG9sWXUxcHR4NUJuYURhbkRJWFE9PQotLS0tLUVORCBDRVJUSUZJQ0FURS0tLS0tCg==";
        private const string RSA2048Key = "LS0tLS1CRUdJTiBQUklWQVRFIEtFWS0tLS0tCk1JSUV2Z0lCQURBTkJna3Foa2lHOXcwQkFRRUZBQVNDQktnd2dnU2tBZ0VBQW9JQkFRREZkckRmc1Yxa3R6M08KWTd2U2c4NjdkOFd0S1RvQzVPMHBOaFpPTU1lQXFWeE1jVUR5WlZGa1dlMmhOSk5SQU1vTU04VkdOcG00Tld4awpVTzhOTDYwb2dRWEpPNnZGcmllSktTQStrSmtGUnh5S1FqbDVWcDBDNE5IWGxGbjZ4eWVTa2lKTTVCeWVtejJECm1kdll5ZkowWjE0bW1NbE1rWUlwMkR4RUQwNmxUR3AwckpuWk92RC9reDhXZ0dTTHg3ZHZ0a2RWdW1JWkpxQW4KSFRUVnVnTlM1NzNyVGZBZjZObS9FSUJkYjUyZ2FQWmVZSDFHTUVxcmNRS0RYUWNxclFlcWpHLzR4VVlOd0EyOQoyekRzU2taVXppTlMzTTJDaHUvMDdldzFoT2FlM3E4R2VtQU1sWjVjMnJHN0I5Vm1vd0tPSTJaeWlTR0xJbUVHCnN5MUFLdnFEQWdNQkFBRUNnZ0VBVVcrRnVudFhYUFlGVTF6bVZRZHU5dm1LV01PcjNRMjBrVTV4SElZWmdRSlgKd2w3aXZoVHk0RVFHTUhtU1Y5SFBnZ0RIMXhFbGpFYmNFUE1mNDN4S0dXNDhpS1ZIVGh0b3lyUmZTKzR1YlhMSwpYbXE1Y1VVN003aXNVaEFvbTdnT3BmQUduVmRtUmpNWHgzSm9ySnQ4bG0wYTh5Z0h2TUExd2pRZmhxSVplU20wCm1NYXVkVnc1NmZuWUNlRHdiOUYwZHhlTm5CZFJEUjhQZHRJVGFVUTdtSlNiMDJaazloMzhZL0xJM3lNOEI5dzAKeGJHRnVJandGZkplUlkwTm9uUkRJKzJoV0dqMWV2d3BTeFpuaFdHZDNlR3VvaGFCWjJJcUF2eVJRVk54M2ZOVQo5d2R2UDN6Ty81UDN5andHVnluR1F4dUtZYnhkdENvUDUwRGhhTHlyQVFLQmdRRDBLMk85SjFyQk9yQnVsKys2CjVCblljNmF0bEUzZmVFeDRWQm1Tak1lQXJlNmNsU1o0SmorVE0wRlgxa1llRis2enRMV1ZyNC9hSlA5ZnhMaSsKV0lFVXF1NVh6aEtzUHN6dmZTNkhMMCtKNUxERzhadXZWVTA1WFJ1Mkh4cW9td1JESU5NZlQ4VDFqWFcyR29mWQo2VnhiMmxwaDhwMmc4bS9HUnhOUVljais0d0tCZ1FEUEIvbVNscEpwaG4wTzhzckNSaTNQdjc3SUlkWXFUM2g2CmIrMFIreUdyeUJlV1ZKUnRYTFQxYzAzclVxWjB5Q21yWnJ6eHVQYjNmT2s5NUNmcERGajdOOVdDcjBkd2d6TGcKOVl4U0x5aXpIMkh1U0pSU1hkYU0rNDFlZE85L1I4OTdoWnRmYjVvQTJ1SkhycldOYzRtOEkwUzl3TlRqcWozTQpvZjFjVjRaSDRRS0JnUUNZQzNUZVV1VldzaDh2bnZrdlhkOGlSRklrZ05kRk12b2JhL05JcU5XS3NjU0dTUEpkCm15TzNuQkV5aTBHNmQ1aXkydkxBTlVUVXlIa2x2N0J6QjFYSjh1N2NTbnlpT2JFUDZOblNZSW1kQ29WeVBQMS8KWHhUcGhhczk2M3lFUlpPUDRaN1dQcXdkWkd0S2JKYnZZZEdiZVFzVE55c2xFQksrVzRBUmEvTG5sUUtCZ0U1bApsYk5WdzFHa2ROemhRYmg1Zk1XTHRZSHhsampsYVJqc0kxakl3VUpjZmRvV1l6SmFoY2ZnaHVRU295ZmIyRE9vCnJ1bldBZWRMN01MY0QrVG5JUmJZV01pbytTTjQreDVHdllFdGpIcnY2bFNpTlZCUGFueHVrV1R3WVBUUXk1ZloKeXpGRGdJek1WaDJRNGYwVGdGZTIvMFBmcm40YnJGVStEa1FXZktuaEFvR0JBTk42dFlPSU1HS0pWaVpVT0c3SwpQOE10TnAxK3FXaHgwTk96b2grb2tVQnoxMnp6OE5aV1B5c2IwUWhWUTBjditxcnZ2Z1k4dUxWTVRzdzdMSktGClZhWElQc0hVVzRtbXFvTjQrS3pHYVdBdFlWeklKS005Zm5vOHppNjFnSEpNZ3dOd0IxTVRhZXU5T1RydWgyaFkKN2JKLzVJQm1pRkVmZlY1ZXlTL3FMNjdXCi0tLS0tRU5EIFBSSVZBVEUgS0VZLS0tLS0K";
        private const string RSA4096Certificate = "LS0tLS1CRUdJTiBDRVJUSUZJQ0FURS0tLS0tCk1JSUVtakNDQW9JQ0NRQ3B6Y29VbFZ6c0t6QU5CZ2txaGtpRzl3MEJBUXNGQURBUE1RMHdDd1lEVlFRRERBUlUKWlhOME1CNFhEVEl5TURZd05qSXpNalF5TmxvWERUTXlNRFl3TXpJek1qUXlObG93RHpFTk1Bc0dBMVVFQXd3RQpWR1Z6ZERDQ0FpSXdEUVlKS29aSWh2Y05BUUVCQlFBRGdnSVBBRENDQWdvQ2dnSUJBTEo1clRhcHFSZ0RuRk9NCjJ1ZVI3eVVYdkQ1cUkydDNuRE95dGZKc1ZmMno5SndvVFlJUVJyYjNxUkdDQnZVcmkweHpFT2xEazRhUWZ4UFgKSU1hSmtEUCtqemRaN3pvUEN0UEZ0bjVka0grNGlYK05jcEptdm0xWkoxWVFKSWlxL3puSDY5VFhSV3ZCSnV4ego0V0M2eTZBT0YvTTRacm9JUk1jRFhuU0xYQ1hORnE5YzNOLzRVSUxDVHZxMjRBbkdUdFZieDVKTkJ2TlkyY1NwCmtmMzhBS3V0VWRGRERITzVSeTdoazB6RkVaUWN1YWZQLzhtWDRkQWQzc1JNTWFwY3F6YkQxRDFoVkEzVUwyaWkKeTdiQmhxT2VqVlpRMXJCdU5IRnJrdTBQQWN6akI4L1pocUNFcjNNbXorY0ZmUjJ5NCtQRXkxdlBGYjd1SFBZbgo1WmNRTnppazFGZzRiSTN2aEJiZGx5djhLem8wZE1OSUFNWTN2Nng5Z0lldGNxRWZOZEhyN1R2OFIzVk5FNmV4ClZ3TXBNNGlFWldaVnljdVRTZDNTZFp4SC9tY2VSdnVhMTh1eTc2Q3ZxVUtrSUhST0ZJZ0F0K3VNRFFkMXlZUTIKdDNhdWFZczdpRCsxNGQ2RFE0eGxMNjBVb3BFeGdKUmFCZUJPYkp5SEU1NDV6U2hjbVVOTHFUdXNyTlVCckQzdwpjTUhQcmpFWmJ3eHJpQzVWYkxYZ1IxTnUvc1BuaHNWamVZN003dWV0ZG1vMXBreWRpOVZORVpjOXNUL1NTWmVnCjdsNjYwTmYxcG04T2FJUWJmZmJLd3BXTkNtTzFTZjRlZVZMK2kvWUhreFlITEpwUVlvUlo0ZFdDNTRPT2p4cU4KR2NJZEZzYTZRaWdhYmQ0WTZrTWpKeXFwZnpIN0FnTUJBQUV3RFFZSktvWklodmNOQVFFTEJRQURnZ0lCQUxFQgoxUXZlZ2tvdnVTUENkQ1RCS0grdGVIY0d3ME8xVHNTRktsTXdlaUJoZndRbi9PczJpQ3NJdEtmSittdEpKazNSCkhqSUtGV2RsdnNhQ0ZHQjU1bGl4NFUxT0FLc0hrV3dWc0c5U3JhWkhnZ0RWdnFua3N3amlwbVloUTVnWUZhanAKcW95L2lncUFTSlA4LzBxY25HdEhtYWlXam54OVc0bHkvMjN1K0J2SzdQZ1JsUU54bzJTa256M2lNcmpicnRCegp0MHZaQ2ZnMzdhakVockFpaVRxUGRLblIzb1p3T3QxUTlld2pORXh4U1VIangyZkhPUWhBOGZUMzc3NUt3OHZKCkhqc2dRbjdyU1FMTnlybGdkSFQ1OWhQS242cW15bEdwMVEyL0pvTlM2WnhHKzJpNHM0ZzlRMjBSWVFuRTI2d2sKeFZiNDRweU9iOXZyNG94WU1RZERYaXRjb0pTdHFCUDl5YXhVRXVnMm5zVm51Q29vQS9SdjRqcmJ1UTdvQURqaQpHY2tSR1lQSkJqS0ZwUGl6SzR4YzRkbmxsNm56bkFhdGY4ZE9jdVdzVWVyZlJ3a2haaTJEbytCL0JBMWYxTStICjRTUEQ5cDFLSlh2SVkrSXlNeTJ6V1BBamxYMXZBY0lzcGRvTURRUkVaVmQvZlpidmtYVFRxRUtDZW91VSszTCsKNncxVHh1VXJOWDQ2MGIzYjEwOU5lQnlsMFRrYkdOL2dhdGxVeUJRSlVvOWQxY0VDWklNQnh1c2d3WlRHcVlJSwpOWkdTZnNIbzc2TFVhZVI0Qk04elZCaUxQT1N2L240dHpIemhldzFEbFN2V3l0anJuaWJTdklNSWRiWVdTQkJPCkowRTBzMlIrN1A4SktOWVFFRXNvNXM1UHZ3SDZFdlZjaDNnZFFKVmwKLS0tLS1FTkQgQ0VSVElGSUNBVEUtLS0tLQo=";
        private const string RSA4096Key = "LS0tLS1CRUdJTiBQUklWQVRFIEtFWS0tLS0tCk1JSUpRZ0lCQURBTkJna3Foa2lHOXcwQkFRRUZBQVNDQ1N3d2dna29BZ0VBQW9JQ0FRQ3llYTAycWFrWUE1eFQKak5ybmtlOGxGN3crYWlOcmQ1d3pzclh5YkZYOXMvU2NLRTJDRUVhMjk2a1JnZ2IxSzR0TWN4RHBRNU9Ha0g4VAoxeURHaVpBei9vODNXZTg2RHdyVHhiWitYWkIvdUlsL2pYS1NacjV0V1NkV0VDU0lxdjg1eCt2VTEwVnJ3U2JzCmMrRmd1c3VnRGhmek9HYTZDRVRIQTE1MGkxd2x6UmF2WE56ZitGQ0N3azc2dHVBSnhrN1ZXOGVTVFFieldObkUKcVpIOS9BQ3JyVkhSUXd4enVVY3U0Wk5NeFJHVUhMbW56Ly9KbCtIUUhkN0VUREdxWEtzMnc5UTlZVlFOMUM5bwpvc3Uyd1lham5vMVdVTmF3YmpSeGE1THREd0hNNHdmUDJZYWdoSzl6SnMvbkJYMGRzdVBqeE10Ynp4Vys3aHoyCkorV1hFRGM0cE5SWU9HeU43NFFXM1pjci9DczZOSFREU0FER043K3NmWUNIclhLaEh6WFI2KzA3L0VkMVRST24Kc1ZjREtUT0loR1ZtVmNuTGswbmQwbldjUi81bkhrYjdtdGZMc3UrZ3I2bENwQ0IwVGhTSUFMZnJqQTBIZGNtRQpOcmQycm1tTE80Zy90ZUhlZzBPTVpTK3RGS0tSTVlDVVdnWGdUbXljaHhPZU9jMG9YSmxEUzZrN3JLelZBYXc5CjhIREJ6NjR4R1c4TWE0Z3VWV3kxNEVkVGJ2N0Q1NGJGWTNtT3pPN25yWFpxTmFaTW5ZdlZUUkdYUGJFLzBrbVgKb081ZXV0RFg5YVp2RG1pRUczMzJ5c0tWalFwanRVbitIbmxTL292MkI1TVdCeXlhVUdLRVdlSFZndWVEam84YQpqUm5DSFJiR3VrSW9HbTNlR09wREl5Y3FxWDh4K3dJREFRQUJBb0lDQVFDTFc3RWh2cVozUnRKNDlzSXpFV05mClhrSXhwaVRVRkVoV29kT3FZR2RndTNSanZxQ096M0M0QzNmcllORlgvTkUvTjVYYjNsVkNQRy9OcVN4QmVrYXMKSXJmbmE0VDltNk5EcXQwTm5MZG8wMG9oMit1N1h4aVFQNDRVaUhST2R0N2xXakxPTmUxUjd6Y2FhSDF5VDBrUwpTQTd4Ym5mZ0NQYlRRdEV0UldnRmFEZXZaRjVmVUFCbHFaMkw3T0hSSUJTTVNxYnhtTTIxbEFvdW9vMkplWTE0CmY5UWtCeXVnN09vbE5DUVZUTWVtRkN2dldkMkxwdG0rN0tQMk9OeUsvdUw5MXpsUldEOE9zVUQzZmk1OUhHMnEKTXZKL2xJMmVvOTYxTlY4N1ZYMTlZMFRGU2oveDlNa2VyQTZvZ0VPcXZmVnQ4MHNxKys4cDc3bVRoNkdoTEJ0QgpCRi94cmtiZHRjL28zdk5IaFk2VHRyMytOVjVJL2o2VHRLTDA4cVQ2djR5enhLQzF0U2hDWTJBZEhSWVArR0VhCjZrNzMrKzhXUHA1T2RyUFJXOHVFc0xreVRPVkgwcllBUU5yckhha0EvbTlJS2ppbytvc3B2RXBzNGZwbXU2YWMKbkdVSzlJelVqaktBdk12Nk01dU9pZTVYS3BHQnk3bTNpTjNJYU15NGh3cGs1MzlaSnBCdjZFWXVPZHhLYjRoVAppck03N0NVZzAxRks1enZNanEvOFdpS2c4aG9SekUzSG0rTC9LRVVBc29uL3BpTThwUTR1RVlqZ2FmNWhDNmtqCmdYLytOWitEdVEwTjZ4RzB0SXh2LzI4L1VHdVJvakduWlBxUGpNdFJzVTdTbkpWWTVPaGZSSTlzdmV6TTMxSUQKM1Jvb2l6WVlXVnZjSm1MSE9HUVRZUUtDQVFFQTMrZ0NoaDdxNDRwR1J5UjMxSFlLank3Wlp1dFE4VlZiZUl2UgpIdWxLMjYzaFRkWG1pY0Z6K0ZIcWk1a0FVTTNkcXF3dzllV055SmdwL2pZL2xMcW85SGg4NWRreFNuSXZCT0U3CnR0OVBGelhkdkNsY3kxSjhPbXd5eG5aclo4MUpuNjgxSWJlUzdKcjN5ZGoyMjU4b2F2T25DZTNMU2NhMnp4b1gKNGl4SFNTYXBSZXU3WThzblM0WGR1d2QzOTRvZDVXcmxONFlndjZrZVdxRHRzNEV6cWRRM29nZnZKb3U0dDNzZQpMb0JndjhkK3cza1hqTUI0Z0kvYjQ4M00vWWVSeW04SEFzUEVJankvc3F2dkNWbE9KOHB3djFNbnVFRUxucmxPCmdaVXpTRUlIeDNKS2pZMERENWZWczRhNVZiY0ZDczZWUUdQY0RQYW5VTElVemJMZjZ3S0NBUUVBekE2Z3A2ancKcTFqR3hmRStPOVovVU9acklXNG5MNnpxdHQ4VmN1V3kzcmN4ZUZnR3RpVHZzRWhWRjNCVnVnK09OVlBOdGR5TwpjaDZqdGVac3hwSUcxSlpWMEtMZkJqNThhZHN6eXVRSWNtekFoYU1XWTJzMHBOcTJuL1V6VTZGcHBsMTI0MDdhCm9QbENKRTFvS3lyUEwrbGlvVlFiWUJhMG5XWXJzbE5iVDhaZHVKNUpVamFxZ0ZONkl4bGExWXB0dWMyS2pid3UKYmh1RzgyRWNwWHN0VHVnYUFEM3IvNU9hdXJla0hqZ3c3MXVVOUdoWU1GcUdFNFhpNHBQNFR1d1BJYkJjbGh1aQp4OG91dm51c3d1M3JKV0NLeVhjN1JPcGNueU81eWE3WHB5c092ZmdJU3hGQW4yTzRPVmVXeGVpcHNTNm91SzJsCmdDZ21HcEljMTJlQ01RS0NBUUE5R05jUGVBSktRcUg0cWJmVkRyekU0MWpKWDY2L203WVJCNzN4ZkdGRDNvZWIKTUtIdXR3NEpGWGsxekhNc21YQkNNU2hQQU91V2NBY05hZ29oY0duSUE5UzQ5M0lleVZlUk1sQ1VEdzFLM1JUNwpVaUlURFJmKzdTVlJ5R0I0ZTZqNlhOblhzSHJ6K3psZ0o4YjhtTXB6bkxiWTI3MWNEVkEyNVB6ZFo0MndKVStOClY4elRaMHo2cTdXY1ltWlc4cVpEd3M1M1B5OVNmVGhlTEltUU1OSkRKamhrQzF3em5XNnpDbkpxNS9iK2ZkWlcKU29XbGo2T1NHUzg4VXh6N2xYR3YxbW9JVkJrQjZxZTRmbmdFaWd6c2lLNEpWd3lBbDZjdkVZdWRpblBxNUxHdwp0N3dtTWZOQzN3Q2VNMFlCWUthTWsyQmdxSjhVSG4zN1pBeVBNSjh2QW9JQkFHVFpuZUQvQitOSGZ3WDVVdTZ6CjlqeC9oTmNDMys4YUlPNjhscGN3bXhTanFabDJ0K3B4bXR5ZkF4OFFiKzRESGgrUkNZd21NMmlIYUJYUWlsWWgKZWFxN21qSUJMUVNqQVU0My9nTDZiSXBRNFYycTJyZk1GanFidGNLY3B2OWdPUnoxK0hvalVMNWFNcXhLYlg5WQpLd25jNk5nZS8yQ2dHQ2ZxaFJJZlRIUEM4REw4Vi9SLy9BaHNPV0w3ZHY4dTZVZjVJODhsQURWKytWVnoyM3FWCnhEREpXWUJoR2pBNFBUS203RC9iT2FReXVRMktQSHU1WWFab2QxZlFGZHE3cEViY0tRWXZKbVpqc0hLSUlUdzkKNWdlVkRVTEE0TlFRbURJMnIrM1RzZFYyM0ZSYkw0NGFPcndMeHRMZ1RTNEc3VWNhYmx6SHhaSHZSZFcvL08wZwp4U0VDZ2dFQUlwVjJWS2kzelllN0ZVYkpuK0RxWS9UZGllZGxEdW9tWUY0cXlTUXhNNnpCSHNTcmxuM1daVHNxCmdBUEhRTHlZRzR1VUlZV0FMMGVmSVBCRHRqdXc1MjhuenlORUFGV0ZpNG9Ldks1djJuR1hvQVdEazJNTnZSemIKODg5dFNtNTVSbG15NW93d3FSSnVHVmh5UmI3ZmVMZ1BLcWwwWFBPRXhkU1dLaDluOUVrK25wYndvdE1Oa3ZVTApPS3l2S2dlekhuR2tSQjRsYVI1bFJJYXFFcUlKVjBDOUZjM2Ezc2ZJK1JzMmZ3SkxRa2k3ODJaUXFIVHNkUnpkCmJxVVBEREsvRUlySkRYN0dvQ3ovQmRKZEpCd3dSVzQrVlNLTGJPd2JHQjJSbjdDeWZHdUpwV3NKR2V6aHNYelMKbkVrTk9QZzJGb085YTdRMEQ1dEtLZ1h6bkZrVzd3PT0KLS0tLS1FTkQgUFJJVkFURSBLRVktLS0tLQo=";

        [Theory]
        [InlineData(ECDSA128Certificate)]
        [InlineData(ECDSA256Certificate)]
        [InlineData(ECDSA512Certificate)]
        [InlineData(RSA1024Certificate)]
        [InlineData(RSA2048Certificate)]
        [InlineData(RSA4096Certificate)]
        public void CreateFromPemData_Public(string certificate)
        {
            using X509Certificate2 cert = X509Cert.CreateFromPemData(ConvertBase64EncodedCertificate(certificate));
            cert.HasPrivateKey.Should().BeFalse();
            cert.Subject.Should().Be("CN=Test");
            cert.PublicKey.Should().NotBeNull();
        }

        // The 128 and 512 ecdsa test are commented out because osx isn't able to load them.
        [Theory]
        // [InlineData(ECDSA128Certificate, ECDSA128Key)]
        [InlineData(ECDSA256Certificate, ECDSA256Key)]
        // [InlineData(ECDSA512Certificate, ECDSA512Key)]
        [InlineData(RSA1024Certificate, RSA1024Key)]
        [InlineData(RSA2048Certificate, RSA2048Key)]
        [InlineData(RSA4096Certificate, RSA4096Key)]
        public void CreateFromPemData_Public_And_Private(string certificate, string key)
        {
            string certData = ConvertBase64EncodedCertificate(certificate);
            string keyData = ConvertBase64EncodedCertificate(key);
            using X509Certificate2 cert = X509Cert.CreateFromPemData(certData, keyData);
            cert.HasPrivateKey.Should().BeTrue();
            cert.Subject.Should().Be("CN=Test");
            cert.PublicKey.Should().NotBeNull();
        }

        private static string ConvertBase64EncodedCertificate(string data)
        {
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(data));
        }
    }
}

#endif