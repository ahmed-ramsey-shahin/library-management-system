using FluentValidation;
using PhoneNumbers;

namespace Lms.Application.Common
{
    public static class FluentValidationExtensions
    {
        private static readonly PhoneNumberUtil _phoneUtil = PhoneNumberUtil.GetInstance();

        public static IRuleBuilderOptions<T, string> IsValidPhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder, string defaultRegion = "EG")
        {
            return ruleBuilder.Must(phoneNumber =>
            {
                if (string.IsNullOrWhiteSpace(phoneNumber)) return false;

                try
                {
                    var numberProto = _phoneUtil.Parse(phoneNumber, defaultRegion);
                    return _phoneUtil.IsValidNumber(numberProto);
                }
                catch (NumberParseException)
                {
                    return false;
                }
            });
        }
    }
}
