using System.ComponentModel;

namespace RimeTyrant.Tools
{
    /// <summary>
    /// UI中的绑定数据
    /// </summary>
    internal class UI : INotifyPropertyChanged
    {
        #region 加词配置

        #region 字段

        private string _wordToAdd = string.Empty;

        private string _wordColor = string.Empty;

        private bool _useAutoEncode = false;

        private bool _usePriority = false;

        private string[] _encodeMethodArray = [];

        // 0：键道6
        private int _encodeMethodIndex = 0;

        private int[] _validCodeLengthArray = [];

        // 是码长序号，不是码长
        private int _codeLengthIndex = 0;

        private string[] _autoCodeArray = [];

        // 是自动编码序号，不是编码
        private int _autoCodeIndex = 0;

        private string _autoCodeColor = string.Empty;

        private string _manualCode = string.Empty;

        private string _manualCodeColor = string.Empty;

        private string _Priority = string.Empty;

        #endregion

        #region 加词框

        public string WordToAdd
        {
            get => _wordToAdd;
            set
            {
                if (_wordToAdd != value)
                {
                    _wordToAdd = value;
                    OnPropertyChanged(nameof(WordToAdd));
                }
            }
        }

        /// <summary>
        /// 加词框里的文字颜色，当空时，使用默认颜色
        /// </summary>
        public string WordColor
        {
            get => _wordColor;
            set
            {
                if (_wordColor != value)
                {
                    _wordColor = value;
                    OnPropertyChanged(nameof(WordColor));
                }
            }
        }

        #endregion

        #region 自动编码

        public bool UseManualEncode => !_useAutoEncode;

        public bool UseAutoEncode
        {
            get => _useAutoEncode;
            set
            {
                if (_useAutoEncode != value)
                {
                    _useAutoEncode = value;
                    OnPropertyChanged(nameof(UseAutoEncode));
                    OnPropertyChanged(nameof(UseManualEncode));
                }
            }
        }

        public bool UsePriority
        {
            get => _usePriority;
            set
            {
                if (_usePriority != value)
                {
                    _usePriority = value;
                    OnPropertyChanged(nameof(UsePriority));
                }
            }
        }

        public string[] EncodeMethodArray
        {
            get => _encodeMethodArray;
            set
            {
                if (_encodeMethodArray != value)
                {
                    _encodeMethodArray = value;
                    OnPropertyChanged(nameof(EncodeMethodArray));
                }
            }
        }

        public int EncodeMethodIndex
        {
            get => _encodeMethodIndex;
            set
            {
                if (_encodeMethodIndex != value)
                {
                    _encodeMethodIndex = value;
                    OnPropertyChanged(nameof(EncodeMethodIndex));
                    OnPropertyChanged(nameof(EncodeMethod));
                }
            }
        }

        public string? EncodeMethod
            => EncodeMethodArray.Length < EncodeMethodIndex - 1
                ? null
                : EncodeMethodArray[EncodeMethodIndex];

        public int[] ValidCodeLengthArray
        {
            get => _validCodeLengthArray;
            set
            {
                if (_validCodeLengthArray != value)
                {
                    _validCodeLengthArray = value;
                    OnPropertyChanged(nameof(ValidCodeLengthArray));
                }
            }
        }

        /// <summary>
        /// 是码长序号，不是码长
        /// </summary>
        public int CodeLengthIndex
        {
            get => _codeLengthIndex;
            set
            {
                if (_codeLengthIndex != value)
                {
                    _codeLengthIndex = value;
                    OnPropertyChanged(nameof(CodeLengthIndex));
                    OnPropertyChanged(nameof(CodeLength));
                }
            }
        }

        public int CodeLength
            => ValidCodeLengthArray.Length < CodeLengthIndex - 1
                ? -1
                : ValidCodeLengthArray[CodeLengthIndex];

        public string[] AutoCodeArray
        {
            get => _autoCodeArray;
            set
            {
                if (_autoCodeArray != value)
                {
                    _autoCodeArray = value;
                    OnPropertyChanged(nameof(AutoCodeArray));
                }
            }
        }

        /// <summary>
        /// 是自动编码序号，不是编码
        /// </summary>
        public int AutoCodeIndex
        {
            get => _autoCodeIndex;
            set
            {
                if (_autoCodeIndex != value)
                {
                    _autoCodeIndex = value;
                    OnPropertyChanged(nameof(AutoCodeIndex));
                }
            }
        }

        public string? AutoCode
            => AutoCodeArray.Length < AutoCodeIndex
                ? null
                : AutoCodeArray[AutoCodeIndex];

        /// <summary>
        /// 自动编码下拉菜单框里的文字颜色，当空时，使用默认颜色
        /// </summary>
        public string AutoCodeColor
        {
            get => _autoCodeColor;
            set
            {
                if (_autoCodeColor != value)
                {
                    _autoCodeColor = value;
                    OnPropertyChanged(nameof(AutoCodeColor));
                }
            }
        }

        private string[] _originAutoCodeArray = [];

        public string[] OriginAutoCodeArray
        {
            get => _originAutoCodeArray;
            set
            {
                if (_originAutoCodeArray != value)
                {
                    _originAutoCodeArray = value;
                    var now = CodeLength == -1
                        ? value.Select(s => new string(s))
                        : value.Select(s => s[..CodeLength]);
                    AutoCodeArray = [.. now.Distinct().Order()];
                }
            }
        }

        #endregion

        #region 手动编码和优先级

        public string ManualCode
        {
            get => _manualCode;
            set
            {
                if (_manualCode != value)
                {
                    _manualCode = value;
                    OnPropertyChanged(nameof(ManualCode));
                }
            }
        }

        /// <summary>
        /// 手动编码框里的文字颜色，当空时，使用默认颜色
        /// </summary>
        public string ManualCodeColor
        {
            get => _manualCodeColor;
            set
            {
                if (_manualCodeColor != value)
                {
                    _manualCodeColor = value;
                    OnPropertyChanged(nameof(ManualCodeColor));
                }
            }
        }

        public string Priority
        {
            get => _Priority;
            set
            {
                if (_Priority != value)
                {
                    _Priority = value;
                    OnPropertyChanged(nameof(Priority));
                }
            }
        }

        #endregion

        #endregion

        #region 加词按钮

        private bool _allowAdd = false;

        public bool AllowAdd
        {
            get => _allowAdd;
            set
            {
                if (_allowAdd != value)
                {
                    _allowAdd = value;
                    OnPropertyChanged(nameof(AllowAdd));
                }
            }
        }

        #endregion

        #region 查找

        private string _codeToSearch = string.Empty;

        public string CodeToSearch
        {
            get => _codeToSearch;
            set
            {
                if (_codeToSearch != value)
                {
                    _codeToSearch = value;
                    OnPropertyChanged(nameof(CodeToSearch));
                }
            }
        }

        private Item[] _originResultArray = [];

        public Item[] OriginResultArray
        {
            get => _originResultArray;
            set
            {
                if (_originResultArray != value)
                {
                    _originResultArray = value;
                    ResultArray = value;
                }
            }
        }

        private Item[] _resultArray = [];

        public Item[] ResultArray
        {
            get => _resultArray;
            set
            {
                if (_resultArray != value)
                {
                    _resultArray = value;
                    OnPropertyChanged(nameof(ResultArray));
                }
            }
        }

        #endregion

        #region 三个编辑按钮

        private bool _allowDel = false;

        private bool _allowCut = false;

        private bool _allowMod = false;

        public bool AllowDel
        {
            get => _allowDel;
            set
            {
                if (_allowDel != value)
                {
                    _allowDel = value;
                    OnPropertyChanged(nameof(AllowDel));
                }
            }
        }

        public bool AllowCut
        {
            get => _allowCut;
            set
            {
                if (_allowCut != value)
                {
                    _allowCut = value;
                    OnPropertyChanged(nameof(AllowCut));
                }
            }
        }

        public bool AllowMod
        {
            get => _allowMod;
            set
            {
                if (_allowMod != value)
                {
                    _allowMod = value;
                    OnPropertyChanged(nameof(AllowMod));
                }
            }
        }

        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
