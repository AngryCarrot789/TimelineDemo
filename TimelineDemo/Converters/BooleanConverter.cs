using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TimelineDemo.Converters {
    [ValueConversion(typeof(bool), typeof(object))]
    public class BooleanConverter : IValueConverter {
        public object TrueValue { get; set; }
        public object FalseValue { get; set; }

        public object InvalidValue { get; set; }
        public object UnsetValue { get; set; }

        public bool UseTrueForInvalid { get; set; }
        public bool UseTrueForUnset { get; set; }

        public bool Invert { get; set; }
        public bool InvertFallback { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is bool state) {
                if (state) {
                    return this.Invert ? this.FalseValue : this.TrueValue;
                }
                else {
                    return this.Invert ? this.TrueValue : this.FalseValue;
                }
            }
            else if (value == DependencyProperty.UnsetValue) {
                if (this.UseTrueForUnset) {
                    return this.InvertFallback ? this.FalseValue : this.TrueValue;
                }
                else {
                    return this.UnsetValue;
                }
            }
            else {
                if (this.UseTrueForInvalid) {
                    return this.InvertFallback ? this.FalseValue : this.TrueValue;
                }
                else {
                    return this.InvalidValue;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}