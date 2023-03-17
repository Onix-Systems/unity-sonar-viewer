
using System;
using System.Threading;
using UnityEngine;

namespace Utils.ProgressReporting
{
    public class ProgressReporter<T> : IProgressReporter<T> where T: ProgressInfo
    {
        private float _minValue;
        private float _maxValue;
        private float _range;
        
        private Action<ProgressReport<T>> onProgress;

        private readonly SynchronizationContext _synchronizationContext;
        private readonly SendOrPostCallback _callback;

        public ProgressReporter(float minValue, float maxValue, Action<ProgressReport<T>> onProgress)
        {
            _minValue = minValue; 
            _maxValue = maxValue;
            _range = maxValue - minValue;
            this.onProgress = onProgress;

            _synchronizationContext = SynchronizationContext.Current;
            _callback = InvokeReport;
        }

        public void Report(T progressInfo)
        {
            _synchronizationContext.Post(_callback, progressInfo);
        }

        private void InvokeReport(object state)
        {
            T val = (T)state;
            Action<ProgressReport<T>> handler = onProgress;

            float progress = Mathf.Clamp01(val.ProgressNormalizedValue);
            float progressValue = _minValue + (_range * progress);

            ProgressReport<T> report = new ProgressReport<T>()
            {
                ProgressValue = progressValue,
                ProgressInfo = val,
            };

            handler?.Invoke(report);
        }
    }
}
