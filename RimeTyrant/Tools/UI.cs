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

        private bool _useAutoEncode = false;

        private bool _usePriority = false;

        private string[] _encodeMethods = [];

        // 0：键道6
        private int _encodeMethodIndex = -1;

        private int[] _validCodeLengths = [];

        // 是码长序号，不是码长
        private int _codeLengthIndex = -1;

        private string[] _shortCodes = [];

        // 是自动编码序号，不是编码
        private int _codeIndex = -1;

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

        public string[] EncodeMethods
        {
            get => _encodeMethods;
            set
            {
                if (_encodeMethods != value)
                {
                    _encodeMethods = value;
                    OnPropertyChanged(nameof(EncodeMethods));
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
            => EncodeMethodIndex >= 0 && EncodeMethods.Length > EncodeMethodIndex
                ? EncodeMethods[EncodeMethodIndex]
                : null;

        public int[] ValidCodeLengths
        {
            get => _validCodeLengths;
            set
            {
                if (_validCodeLengths != value)
                {
                    _validCodeLengths = value;
                    OnPropertyChanged(nameof(ValidCodeLengths));
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
                    ShortCodes = CodeLength != -1
                                    && encoder.CutCodes(FullCodes, CodeLength, out var now)
                                        ? now : [];
                    OnPropertyChanged(nameof(CodeLengthIndex));
                }
            }
        }

        public int CodeLength
            => CodeLengthIndex >= 0 && ValidCodeLengths.Length > CodeLengthIndex
                ? ValidCodeLengths[CodeLengthIndex]
                : -1;

        public string[] ShortCodes
        {
            get => _shortCodes;
            private set
            {
                if (_shortCodes != value)
                {
                    var prev = AutoCode;
                    _shortCodes = value;
                    OnPropertyChanged(nameof(ShortCodes));
                    // 根据上次的选择来确定这次的选择
                    CodeIndex = value.Length switch
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
        public int CodeIndex
        {
            get => _codeIndex;
            set
            {
                if (_codeIndex != value)
                {
                    _codeIndex = value;
                    OnPropertyChanged(nameof(CodeIndex));
                }
            }
        }

        public string? AutoCode
            => CodeIndex >= 0 && ShortCodes.Length > CodeIndex
                ? ShortCodes[CodeIndex]
                : null;

        private string[] _fullCodes = [];

        public string[] FullCodes
        {
            get => _fullCodes;
            set
            {
                if (_fullCodes != value)
                {
                    _fullCodes = value;
                    ShortCodes = CodeLength != -1
                                    && encoder.CutCodes(value, CodeLength, out var now)
                                        ? now : [];
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
                    AllowDel = false;
                    AllowCut = false;
                }
            }
        }

        private async void Search(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                var result = await SearchAsync(code);
                _ = await MainThread.InvokeOnMainThreadAsync(() => OriginResults = result);
            }
            else OriginResults = [];
        }

        private static async Task<Item[]> SearchAsync(string code)
            => await Task.Run(()
                => Dict.CodeStartsWith(code).OrderBy(x => x.Code).ToArray());

        private Item[] _originResults = [];

        public Item[] OriginResults
        {
            get => _originResults;
            set
            {
                if (_originResults != value)
                {
                    _originResults = value;
                    ViewResults = value.Select(e => e.Clone()).ToArray();
                }
            }
        }

        private Item[] _viewResults = [];

        public Item[] ViewResults
        {
            get => _viewResults;
            private set
            {
                if (_viewResults != value)
                {
                    _viewResults = value;
                    OnPropertyChanged(nameof(ViewResults));
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
