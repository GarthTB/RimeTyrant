using System.ComponentModel;

namespace RimeTyrant.Tools
{
    /// <summary>
    /// UI中的绑定数据
    /// </summary>
    internal class UI : INotifyPropertyChanged
    {
        // 始终和主页保持同一个编码器
        public Encoder encoder = new();

        #region 加词配置

        #region 字段

        private string _wordToAdd = string.Empty;

        private string _wordColor = string.Empty;

        private bool _useAutoEncode = false;

        private bool _usePriority = false;

        private string[] _encodeMethodArray = [];

        // 0：键道6
        private int _encodeMethodIndex = -1;

        private int[] _validCodeLengthArray = [];

        // 是码长序号，不是码长
        private int _codeLengthIndex = -1;

        private string[] _autoCodeArray = [];

        // 是自动编码序号，不是编码
        private int _autoCodeIndex = -1;

        private string _autoCodeColor = string.Empty;

        private string _manualCode = string.Empty;

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
        /// 加词框里的文字颜色，有该词则变红
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
                }
            }
        }

        public string? EncodeMethod
            => EncodeMethodIndex >= 0 && EncodeMethodArray.Length > EncodeMethodIndex
                ? EncodeMethodArray[EncodeMethodIndex]
                : null;

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
                    AutoCodeArray = CodeLength != -1
                                    && encoder.Shorten(OriginAutoCodeArray, CodeLength, out var now)
                                        ? now : [];
                    OnPropertyChanged(nameof(CodeLengthIndex));
                }
            }
        }

        public int CodeLength
            => CodeLengthIndex >= 0 && ValidCodeLengthArray.Length > CodeLengthIndex
                ? ValidCodeLengthArray[CodeLengthIndex]
                : -1;

        public string[] AutoCodeArray
        {
            get => _autoCodeArray;
            private set
            {
                if (_autoCodeArray != value)
                {
                    var prev = AutoCode;
                    _autoCodeArray = value;
                    OnPropertyChanged(nameof(AutoCodeArray));
                    // 根据上次的选择来确定这次的选择
                    AutoCodeIndex = value.Length switch
                    {
                        0 => -1,
                        1 => 0,
                        _ => prev is null
                             ? 0
                             : value.Select((str, idx) => new { str, idx })
                                    .FirstOrDefault(item => item.str.StartsWith(prev) || prev.StartsWith(item.str))
                                    ?.idx ?? 0
                    };
                    CodeToSearch = AutoCode ?? string.Empty;
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
            => AutoCodeIndex >= 0 && AutoCodeArray.Length > AutoCodeIndex
                ? AutoCodeArray[AutoCodeIndex]
                : null;

        /// <summary>
        /// 自动编码下拉菜单框里的文字颜色，有多项则变红
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
                    AutoCodeArray = CodeLength != -1
                                    && encoder.Shorten(value, CodeLength, out var now)
                                        ? now : [];
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
                    Search(value);
                }
            }
        }

        private async void Search(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                var result = await SearchAsync(code);
                _ = await MainThread.InvokeOnMainThreadAsync(() => OriginResultArray = result);
            }
            else OriginResultArray = [];
        }

        private static async Task<Item[]> SearchAsync(string code)
            => await Task.Run(()
                => Dict.CodeStartsWith(code).OrderBy(x => x.Code).ToArray());

        private Item[] _originResultArray = [];

        public Item[] OriginResultArray
        {
            get => _originResultArray;
            private set
            {
                if (_originResultArray != value)
                {
                    _originResultArray = value;
                    ResultArray = value.Select(e => e.Clone()).ToArray();
                }
            }
        }

        private Item[] _resultArray = [];

        public Item[] ResultArray
        {
            get => _resultArray;
            private set
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
