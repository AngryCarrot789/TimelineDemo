namespace TimelineDemo {
    public class TimelineElementMoveData {
        public bool IsCopyDrop { get; set; }
        public int FrameBegin { get; set; }

        /// <summary>
        /// A copy of the original clip that was dragged, which would be located at <see cref="FrameBegin"/>
        /// </summary>
        public TimelineElementControl CopiedElement { get; set; }

        public TimelineElementControl OriginalElement { get; }

        public TimelineElementMoveData(TimelineElementControl originalElement) {
            this.OriginalElement = originalElement;
        }

        public void OnDragComplete() {
            if (this.IsCopyDrop) {
                // Move the copied clip to where the mouse is, and put the
                // original clip (this instance) back to where it originally was
                this.CopiedElement.FrameBegin = this.FrameBegin;
                this.FrameBegin = this.FrameBegin;
                this.OriginalElement.TimelineLayer.OnClipDragged(this.CopiedElement, this);
            }
            else {
                this.OriginalElement.TimelineLayer.OnClipDragged(this.OriginalElement, this);
            }
        }

        public void OnDragCancelled() {
            this.DestroyCopiedClip();
            this.OriginalElement.FrameBegin = this.FrameBegin;
        }

        public void DestroyCopiedClip() {
            if (this.CopiedElement != null) {
                this.OriginalElement.TimelineLayer.DestroyClip(this.CopiedElement);
                this.CopiedElement = null;
            }
        }
    }
}