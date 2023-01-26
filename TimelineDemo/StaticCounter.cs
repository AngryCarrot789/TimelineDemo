namespace TimelineDemo {
    public class StaticCounter {
        public static StaticCounter Instance { get; } = new StaticCounter();

        private int nextNumber;
        public int NextNumber => this.nextNumber++;
    }
}