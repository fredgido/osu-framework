﻿using System;
using osu.Framework.Configuration;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Transformations;
using osu.Framework.Input;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

namespace osu.Framework.Graphics.UserInterface
{
    public abstract class SliderBar<T> : Container where T : struct,
        IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
    {
        public float KeyboardStep { get; set; } = -1;

        public BindableNumber<T> Bindable
        {
            get { return bindable; }
            set
            {
                if (bindable != null)
                    bindable.ValueChanged -= bindableValueChanged;
                bindable = value;
                bindable.ValueChanged += bindableValueChanged;
                UpdateValue(NormalizedValue);
            }
        }
        
        protected float NormalizedValue
        {
            get
            {
                if (Bindable == null)
                    return 0;
                var min = Convert.ToSingle(Bindable.MinValue);
                var max = Convert.ToSingle(Bindable.MaxValue);
                var val = Convert.ToSingle(Bindable.Value);
                return (val - min) / (max - min);
            }
        }
        
        private BindableNumber<T> bindable;

        protected abstract void UpdateValue(float value);
        
        protected override void Dispose(bool isDisposing)
        {
            if (Bindable != null)
                Bindable.ValueChanged -= bindableValueChanged;
            base.Dispose(isDisposing);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            UpdateValue(NormalizedValue);
        }

        protected override bool OnClick(InputState state)
        {
            handleMouseInput(state);
            return true;
        }

        protected override bool OnDrag(InputState state)
        {
            handleMouseInput(state);
            return true;
        }

        protected override bool OnDragStart(InputState state) => true;

        protected override bool OnDragEnd(InputState state) => true;

        protected override bool OnKeyDown(InputState state, KeyDownEventArgs args)
        {
            var step = KeyboardStep;
            if (step == -1)
                step = (Convert.ToSingle(Bindable.MaxValue) - Convert.ToSingle(Bindable.MinValue)) / 20;
            if (Bindable.IsInteger)
                step = (float)Math.Ceiling(step);
            switch (args.Key)
            {
                case Key.Right:
                    Bindable.Add(step);
                    return true;
                case Key.Left:
                    Bindable.Add(-step);
                    return true;
                default:
                    return false;
            }
        }

        private void bindableValueChanged(object sender, EventArgs e) => UpdateValue(NormalizedValue);

        private void handleMouseInput(InputState state)
        {
            var xPosition = ToLocalSpace(state.Mouse.NativeState.Position).X;
            Bindable.SetProportional(xPosition / DrawWidth);
        }
    }
}
