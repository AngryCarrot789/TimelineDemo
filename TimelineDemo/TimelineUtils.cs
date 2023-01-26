namespace TimelineDemo {
    public static class TimelineUtils {
        public const double MinUnitZoom = 0.0001d;

        public static object ValidateNonNegativeDouble(object value) {
            return (double) value < 0d ? 0d : value;
        }

        public static object ClampUnitZoom(object width) {
            return (double) width < MinUnitZoom ? MinUnitZoom : width;
        }
    }
}
