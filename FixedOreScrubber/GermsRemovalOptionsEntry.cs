using System.Reflection;

using PeterHan.PLib;
using PeterHan.PLib.Options;
using PeterHan.PLib.UI;

using TMPro;
using UnityEngine;

namespace OniMods.FixedOreScrubber
{
    public class GermsRemovalOptionsEntry : IDynamicOption
    {
        /// <summary>
		/// The value in the text field.
		/// </summary>
		private int value;

        public object Value
        {
            get { return value; }
            set
            {
                if (value is int newValue)
                {
                    this.value = newValue;
                    Update();
                }
            }
        }

        public string Category
        {
            get; private set;
        }

        public string Title
        {
            get { return "Disease removal count"; }
        }

        public string ToolTip
        {
            get { return "Amount of germs that will be removed per use."; }
        }
        /// <summary>
        /// The realized text field.
        /// </summary>
        private GameObject textField;

        /// <summary>
        /// The realized slider field
        /// </summary>
        private GameObject slider;


        public GermsRemovalOptionsEntry()
        {
            textField = null;
            slider = null;
            value = 0;
            Category = string.Empty;
        }

        public GermsRemovalOptionsEntry(string category)
        {
            textField = null;
            slider = null;
            value = 0;
            Category = category;
        }

        internal static readonly RectOffset SLIDER_MARGIN = new RectOffset(10, 10, 0, 0);
        internal static readonly RectOffset ENTRY_MARGIN = new RectOffset(15, 0, 0, 5);

        /// <summary>
		/// The limits allowed for the entry.
		/// </summary>
		protected readonly LimitAttribute limits = new LimitAttribute(1, 2000000);


        public GameObject GetUIComponent()
        {
            double minLimit = limits.Minimum;
            double maxLimit = limits.Maximum;

            PGridPanel grid = new PGridPanel();

            grid.AddColumn(new GridColumnSpec());


            PTextField textFieldControl = new PTextField()
            {
                OnTextChanged = OnTextChanged,
                //ToolTip = LookInStrings(ToolTip),
                Text = value.ToString(),
                MinWidth = 64,
                MaxLength = 10,
                Type = PTextField.FieldType.Integer
            };


            PSliderSingle sliderControl = new PSliderSingle()
            {
                 MinValue = (float)minLimit,
                 MaxValue = (float)maxLimit,
                 IntegersOnly = true,
                 InitialValue = value,
                 OnValueChanged = OnSliderChanged,
            };

            // Min and max labels
            var minLabel = new PLabel("MinValue")
            {
                TextStyle = PUITuning.Fonts.TextLightStyle,
                Text = minLimit.ToString("G4"),
                TextAlignment = TextAnchor.MiddleRight
            };
            var maxLabel = new PLabel("MaxValue")
            {
                TextStyle = PUITuning.Fonts.TextLightStyle,
                Text = maxLimit.ToString("N0"),
                TextAlignment = TextAnchor.MiddleLeft
            };
            // Lay out left to right
            var panel = new PRelativePanel("Slider Grid")
            {
                FlexSize = Vector2.right,
                DynamicSize = true
            }.AddChild(sliderControl).AddChild(minLabel).AddChild(maxLabel).AnchorYAxis(sliderControl).
                AnchorYAxis(minLabel, 0.5f).AnchorYAxis(maxLabel, 0.5f).SetLeftEdge(
                minLabel, fraction: 0.0f).SetRightEdge(maxLabel, fraction: 1.0f).
                SetLeftEdge(sliderControl, toRight: minLabel).SetRightEdge(sliderControl, toLeft:
                maxLabel).SetMargin(sliderControl, SLIDER_MARGIN);

            sliderControl.OnRealize += OnRealizeSlider;
            textFieldControl.OnRealize += TextFieldControl_OnRealize;


            grid.AddRow(new GridRowSpec());           
            grid.AddRow(new GridRowSpec());
            grid.AddChild(textFieldControl, new GridComponentSpec(0, 0));
            grid.AddChild(panel, new GridComponentSpec(1, 0) { Margin = ENTRY_MARGIN });


            //Update();
            return grid.Build();
        }

        private void TextFieldControl_OnRealize(GameObject realized)
        {
            textField = realized;
            Update();
        }


        /// <summary>
        /// Called when the input field's text is changed.
        /// </summary>
        /// <param name="text">The new text.</param>
        private void OnTextChanged(GameObject _, string text)
        {     
            if (int.TryParse(text, out int newValue))
            {
               if (limits != null)
                    newValue = limits.ClampToRange(newValue);

                // Record the valid value
                value = newValue;
            }
            Update();
        }

        /// <summary>
		/// Called when the slider is realized.
		/// </summary>
		/// <param name="realized">The actual slider.</param>
		private void OnRealizeSlider(GameObject realized)
        {
            slider = realized;
            Update();
        }



        private void OnSliderChanged(GameObject _, float newValue)
        {
            int sliderstep = 1000;

            int newIntValue = Mathf.RoundToInt(newValue);
            if (limits != null)
                newIntValue = limits.ClampToRange(newIntValue);

            float newStep = newIntValue / sliderstep;
           
            value = (int)newStep * sliderstep;
            Update();
        }



        /// <summary>
        /// Updates the displayed value.
        /// </summary>
        private void Update()
        {
            var field = textField?.GetComponentInChildren<TMP_InputField>();

            if (field != null)
                field.text = value.ToString();

            if(slider!= null)
                PSliderSingle.SetCurrentValue(slider, value);
        }
    }


    public class CustomIntOptionsEntry2 : SlidingBaseOptionsEntry
    {
        /// <summary>
		/// The value in the text field.
		/// </summary>
		private int value;

        public override object Value
        {
            get { return value; }
            set
            {
                if (value is int newValue)
                {
                    this.value = newValue;
                    Update();
                }
            }
        }

        /// <summary>
        /// The realized text field.
        /// </summary>
        private GameObject textField;


        public CustomIntOptionsEntry2()
            :base("Test", "Test ToolTip", string.Empty, null)
        {
            textField = null;
            slider = null;
            value = 0;
        }


        public CustomIntOptionsEntry2(string category = "", LimitAttribute limits = null)
            : base("Test", "Test ToolTip", category, limits)
        {
            textField = null;
            slider = null;
            value = 0;
        }


        /*
        protected CustomIntOptionsEntry(string title, string tooltip, string category = "", LimitAttribute limits = null)
            : base(title, tooltip, category, limits)
        {
            textField = null;
            value = 0;
        }

        internal CustomIntOptionsEntry(OptionAttribute oa, PropertyInfo prop)
            : base(oa, prop)
        {
            textField = null;
            value = 0;
        }*/


        /// <summary>
        /// Updates the displayed value.
        /// </summary>
        protected override void Update()
        {
            var field = textField?.GetComponentInChildren<TMP_InputField>();

            if (field != null)
                field.text = value.ToString();

            if (slider != null)
                PSliderSingle.SetCurrentValue(slider, value);
        }


        public override GameObject GetUIComponent()
        {
            slider = new PSliderSingle()
            {
                OnValueChanged = OnSliderChanged,
                ToolTip = ToolTip,
                MinValue = (float)limits.Minimum,
                MaxValue = (float)limits.Maximum,
                InitialValue = value,
                IntegersOnly = true
            }.Build();

            textField = new PTextField()
            {
                OnTextChanged = OnTextChanged,
                //ToolTip = LookInStrings(ToolTip),
                Text = value.ToString(),
                MinWidth = 64,
                MaxLength = 10,
                Type = PTextField.FieldType.Integer
            }.Build();

            Update();
            return slider;
        }

        /// <summary>
        /// Called when the input field's text is changed.
        /// </summary>
        /// <param name="text">The new text.</param>
        private void OnTextChanged(GameObject _, string text)
        {
            if (int.TryParse(text, out int newValue))
            {
                if (limits != null)
                   newValue = limits.ClampToRange(newValue);

                // Record the valid value
                value = newValue;
            }
            Update();
        }

        /// <summary>
		/// Called when the slider's value is changed.
		/// </summary>
		/// <param name="newValue">The new slider value.</param>
		private void OnSliderChanged(GameObject _, float newValue)
        {
            int newIntValue = Mathf.RoundToInt(newValue);
            if (limits != null)
                newIntValue = limits.ClampToRange(newIntValue);
            // Record the value
            value = newIntValue;
            Update();
        }
       

        protected override PSliderSingle GetSlider()
        {
            return new PSliderSingle()
            {
                OnValueChanged = OnSliderChanged,
              //  ToolTip = LookInStrings(ToolTip),
                MinValue = (float)limits.Minimum,
                MaxValue = (float)limits.Maximum,
                InitialValue = value,
                IntegersOnly = true
            };
        }


        /*

        public override void CreateUIEntry(PGridPanel parent, ref int row)
        {
            double minLimit, maxLimit;
            base.CreateUIEntry(parent, ref row);
            if (limits != null && (minLimit = limits.Minimum) > float.MinValue && (maxLimit =
                    limits.Maximum) < float.MaxValue && maxLimit > minLimit)
            {
                // NaN will be false on either comparison
                var slider = GetSlider();
                // Min and max labels
                var minLabel = new PLabel("MinValue")
                {
                    TextStyle = PUITuning.Fonts.TextLightStyle,
                    Text = minLimit.
                    ToString("G4"),
                    TextAlignment = TextAnchor.MiddleRight
                };
                var maxLabel = new PLabel("MaxValue")
                {
                    TextStyle = PUITuning.Fonts.TextLightStyle,
                    Text = maxLimit.
                    ToString("G4"),
                    TextAlignment = TextAnchor.MiddleLeft
                };
                // Lay out left to right
                var panel = new PRelativePanel("Slider Grid")
                {
                    FlexSize = Vector2.right,
                    DynamicSize = false
                }.AddChild(slider).AddChild(minLabel).AddChild(maxLabel).AnchorYAxis(slider).
                    AnchorYAxis(minLabel, 0.5f).AnchorYAxis(maxLabel, 0.5f).SetLeftEdge(
                    minLabel, fraction: 0.0f).SetRightEdge(maxLabel, fraction: 1.0f).
                    SetLeftEdge(slider, toRight: minLabel).SetRightEdge(slider, toLeft:
                    maxLabel).SetMargin(slider, SLIDER_MARGIN);
                slider.OnRealize += OnRealizeSlider;
                // Add another row for the slider
                parent.AddRow(new GridRowSpec());
                parent.AddChild(panel, new GridComponentSpec(++row, 0)
                {
                    ColumnSpan = 2,
                    Margin = ENTRY_MARGIN
                });
            }
        }*/
    }



}
