namespace QLKhachSanAPI.Extensions
{
    public static class TimeHelper
    {
        public static string GetTimeDiff(DateTime? datetimesince)
        {
            if (datetimesince == null)
            {
                return "Invalid datetime!";
            }

            // Calculate the time since the old password was last used
            var diff = DateTime.UtcNow - datetimesince;


            if (diff < TimeSpan.FromSeconds(10))
            {
                return "just now";
            }
            else if (diff < TimeSpan.FromMinutes(1))
            {
                return "a few seconds ago";
            }
            else if (diff < TimeSpan.FromHours(1))
            {
                return "about " + diff?.Minutes + " minutes ago";
            }
            else if (diff < TimeSpan.FromDays(1))
            {
                return "about " + diff?.Hours + " hours ago";
            }
            else if (diff < TimeSpan.FromDays(30))
            {
                return "about " + diff?.Days + " days ago";
            }
            else if (diff < TimeSpan.FromDays(365))
            {
                return "about " + (diff?.Days / 30) + " months ago";
            }
            else
            {
                return "more than a year ago";
            }

        }

        public static string GetTimeDiff(DateTimeOffset? datetimesince)
        {
            if (datetimesince == null)
            {
                return "Invalid datetime!";
            }

            // Calculate the time since the old password was last used
            var diff = DateTime.UtcNow - datetimesince;


            if (diff < TimeSpan.FromSeconds(20))
            {
                return "just now";
            }
            else if (diff < TimeSpan.FromMinutes(1))
            {
                return "a few seconds ago";
            }
            else if (diff < TimeSpan.FromHours(1))
            {
                return "about " + diff?.Minutes + " minutes ago";
            }
            else if (diff < TimeSpan.FromDays(1))
            {
                return "about " + diff?.Hours + " hours ago";
            }
            else if (diff < TimeSpan.FromDays(30))
            {
                return "about " + diff?.Days + " days ago";
            }
            else if (diff < TimeSpan.FromDays(365))
            {
                return "about " + (diff?.Days / 30) + " months ago";
            }
            else
            {
                return "more than a year ago";
            }

        }

        public static string ConvertToUserFriendlyDateTimeFormat(DateTime dateTime)
        {
            string formattedDate = dateTime.ToString("dddd, dd/MM/yyyy hh:mm tt");
            return formattedDate;
        }

        public static string ConvertToUserFriendlyDateTimeFormat(DateTimeOffset dateTimeOffset)
        {
            string formattedDate = $"{dateTimeOffset.ToString("dddd, dd/MM/yyyy hh:mm tt")} (GMT{dateTimeOffset.Offset.Hours:+0;-0}) ({TimeHelper.GetTimeDiff(dateTimeOffset)})";
            return formattedDate;
        }
    }
}
