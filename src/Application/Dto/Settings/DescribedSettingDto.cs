namespace Application.Dto.Settings
{
    public class DescribedSettingDto : SettingDto
    {
        public string DefaultValue { get; set; }

        public string Description { get; set; }
    }
}
