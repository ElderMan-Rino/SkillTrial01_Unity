// [STUB] BootstrapLocaleSheet 데이터가 dll에 포함되면 이 파일 삭제
#if !ELDER_DATA_RUNTIME_HAS_BOOTSTRAP_LOCALE_SHEET
using Unity.Entities;

namespace Elder.SkillTrial.Resources.Data
{
    public struct BootstrapLocaleSheetRoot
    {
        public BlobArray<BootstrapLocaleSheetRow> Rows;
    }

    public struct BootstrapLocaleSheetRow
    {
        public int Id;
        public LanguageType LocaleType;
        public BlobString SheetName;
    }
}
#endif
