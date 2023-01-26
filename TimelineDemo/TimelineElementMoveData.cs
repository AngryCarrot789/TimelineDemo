using TimelineDemo.Timeline;

namespace TimelineDemo {
    public class TimelineElementMoveData {
        /// <summary>
        /// The drag copied the original clip onto the cursor, unless <see cref="IsCopyDropAndMoveOriginal"/> in
        /// which case the copy was dropped at the cursor and the original is is being dragged
        /// </summary>
        public bool IsCopyDropAndLeaveOriginal { get; set; }

        /// <summary>
        /// The drag copied an element at the cursor location and also moved the
        /// original clip somewhere; <see cref="FrameBegin"/> is no longer in use
        /// </summary>
        public bool IsCopyDropAndMoveOriginal { get; set; }

        /// <summary>
        /// The frame at which the drag was started
        /// </summary>
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
            if (this.IsCopyDropAndLeaveOriginal) {
                if (this.IsCopyDropAndMoveOriginal) {
                    // Swap original and copy's positions
                    int copyFrameBegin = this.CopiedElement.FrameBegin;
                    this.CopiedElement.FrameBegin = this.OriginalElement.FrameBegin;
                    this.OriginalElement.FrameBegin = copyFrameBegin;
                }
                else {
                    // Move the copied clip to where the mouse is, and put the
                    // original clip back to where it originally was
                    this.CopiedElement.FrameBegin = this.OriginalElement.FrameBegin;
                    this.OriginalElement.FrameBegin = this.FrameBegin;
                }

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