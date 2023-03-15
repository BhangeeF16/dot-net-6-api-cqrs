﻿using Domain.Common.Extensions;
using Domain.Entities.GeneralModule;
using Domain.Entities.UsersModule;

namespace Infrastructure.Persistence;

public static class SeedData
{
    private static readonly User user = new()
    {
        ID = 1,
        FirstName = "Application",
        LastName = "Admin",
        Email = "admin@reporteq.com",
        Password = PasswordHasher.GeneratePasswordHash("Admin123!"),
        DOB = new DateTime(1999, 10, 19, 0, 0, 0, 0, DateTimeKind.Utc),
        PhoneNumber = "1234567890",
        fk_GenderID = 1,
        fk_RoleID = 1,
        LoginAttempts = 0,
        IsNewsLetter = false,
        IsOTPLogin = false,
        IsPasswordChanged = true,
        IsActive = true,
        IsDeleted = false,
    };
    private static readonly List<Role> roles = new()
    {
        new Role { ID = 1, RoleName = "Admin", IsActive = true, IsDeleted = false },
        new Role { ID = 2, RoleName = "User" , IsActive = true, IsDeleted = false }
    };
    private static readonly List<Gender> genders = new()
    {
        new Gender { ID = 1, Value = "Male" },
        new Gender { ID = 2, Value = "FeMale" },
        new Gender { ID = 3, Value = "Other" },
    };
    private static readonly List<Country> countries = new()
    {
      new Country { ID = 1, Name = "Afghanistan", Abbreviation = "AF" },
      new Country { ID = 2, Name =  "Aland Islands", Abbreviation =  "AX" },
      new Country { ID = 3, Name = "Albania", Abbreviation = "AL" },
      new Country { ID = 4, Name = "Algeria", Abbreviation = "DZ" },
      new Country { ID = 5, Name = "American Samoa", Abbreviation = "AS" },
      new Country { ID = 6, Name = "Andorra", Abbreviation = "AD" },
      new Country { ID = 7, Name = "Angola", Abbreviation = "AO" },
      new Country { ID = 8, Name = "Anguilla", Abbreviation = "AI" },
      new Country { ID = 9, Name = "Antarctica", Abbreviation = "AQ" },
      new Country { ID = 10, Name = "Antigua And Barbuda", Abbreviation = "AG" },
      new Country { ID = 11, Name = "Argentina", Abbreviation = "AR" },
      new Country { ID = 12, Name = "Armenia", Abbreviation = "AM" },
      new Country { ID = 13, Name = "Aruba", Abbreviation = "AW" },
      new Country { ID = 14, Name = "Australia", Abbreviation = "AU" },
      new Country { ID = 15, Name = "Austria", Abbreviation = "AT" },
      new Country { ID = 16, Name = "Azerbaijan", Abbreviation = "AZ" },
      new Country { ID = 17, Name = "Bahamas", Abbreviation = "BS" },
      new Country { ID = 18, Name = "Bahrain", Abbreviation = "BH" },
      new Country { ID = 19, Name = "Bangladesh", Abbreviation = "BD" },
      new Country { ID = 20, Name = "Barbados", Abbreviation = "BB" },
      new Country { ID = 21, Name = "Belarus", Abbreviation = "BY" },
      new Country { ID = 22, Name = "Belgium", Abbreviation = "BE" },
      new Country { ID = 23, Name = "Belize", Abbreviation = "BZ" },
      new Country { ID = 24, Name = "Benin", Abbreviation = "BJ" },
      new Country { ID = 25, Name = "Bermuda", Abbreviation = "BM" },
      new Country { ID = 26, Name = "Bhutan", Abbreviation = "BT" },
      new Country { ID = 27, Name = "Bolivia", Abbreviation = "BO" },
      new Country { ID = 28, Name = "Bosnia  And Herzegovina", Abbreviation = "BA" },
      new Country { ID = 29, Name = "Botswana", Abbreviation = "BW" },
      new Country { ID = 30, Name = "Bouvet Island", Abbreviation = "BV" },
      new Country { ID = 31, Name = "Brazil", Abbreviation = "BR" },
      new Country { ID = 32, Name = "British Indian Ocean Territory", Abbreviation = "IO" },
      new Country { ID = 33, Name = "British Virgin Islands", Abbreviation = "VG" },
      new Country { ID = 34, Name = "Brunei Darussalam", Abbreviation = "BN" },
      new Country { ID = 35, Name = "Bulgaria", Abbreviation = "BG" },
      new Country { ID = 36, Name = "Burkina Faso", Abbreviation = "BF" },
      new Country { ID = 37, Name = "Burundi", Abbreviation = "BI" },
      new Country { ID = 38, Name = "Cambodia", Abbreviation = "KH" },
      new Country { ID = 39, Name = "Cameroon", Abbreviation = "CM" },
      new Country { ID = 40, Name = "Canada", Abbreviation = "CA" },
      new Country { ID = 41, Name = "Canary Islands", Abbreviation = "IC" },
      new Country { ID = 42, Name = "Cape Verde", Abbreviation = "CV" },
      new Country { ID = 43, Name = "Cayman Islands", Abbreviation = "KY" },
      new Country { ID = 44, Name = "Central African Republic", Abbreviation = "CF" },
      new Country { ID = 45, Name = "Chad", Abbreviation = "TD" },
      new Country { ID = 46, Name = "Chile", Abbreviation = "CL" },
      new Country { ID = 47, Name = "China", Abbreviation = "CN" },
      new Country { ID = 48, Name = "Christmas Island", Abbreviation = "CX" },
      new Country { ID = 49, Name = "Cocos (Keeling) Islands", Abbreviation = "CC" },
      new Country { ID = 50, Name = "Colombia", Abbreviation = "CO" },
      new Country { ID = 51, Name = "Comoros", Abbreviation = "KM" },
      new Country { ID = 52, Name = "Congo", Abbreviation = "CG" },
      new Country { ID = 53, Name = "Congo (Democratic Republic)", Abbreviation = "CD" },
      new Country { ID = 54, Name = "Cook Islands", Abbreviation = "CK" },
      new Country { ID = 55, Name = "Costa Rica", Abbreviation = "CR" },
      new Country { ID = 56, Name = "Croatia", Abbreviation = "HR" },
      new Country { ID = 57, Name = "Cuba", Abbreviation = "CU" },
      new Country { ID = 58, Name = "Curaçao", Abbreviation = "CW" },
      new Country { ID = 59, Name = "Cyprus", Abbreviation = "CY" },
      new Country { ID = 60, Name = "Czech Republic", Abbreviation = "CZ" },
      new Country { ID = 61, Name = "Côte D ivoire", Abbreviation = "CI" },
      new Country { ID = 62, Name = "Denmark", Abbreviation = "DK" },
      new Country { ID = 63, Name = "Djibouti", Abbreviation = "DJ" },
      new Country { ID = 64, Name = "Dominica", Abbreviation = "DM" },
      new Country { ID = 65, Name = "Dominican Republic", Abbreviation = "DO" },
      new Country { ID = 66, Name = "Ecuador", Abbreviation = "EC" },
      new Country { ID = 67, Name = "Egypt", Abbreviation = "EG" },
      new Country { ID = 68, Name = "El Salvador", Abbreviation = "SV" },
      new Country { ID = 69, Name = "Equatorial Guinea", Abbreviation = "GQ" },
      new Country { ID = 70, Name = "Eritrea", Abbreviation = "ER" },
      new Country { ID = 71, Name = "Estonia", Abbreviation = "EE" },
      new Country { ID = 72, Name = "Ethiopia", Abbreviation = "ET" },
      new Country { ID = 73, Name = "Falkland Islands (Malvinas)", Abbreviation = "FK" },
      new Country { ID = 74, Name = "Faroe Islands", Abbreviation = "FO" },
      new Country { ID = 75, Name = "Fiji", Abbreviation = "FJ" },
      new Country { ID = 76, Name = "Finland", Abbreviation = "FI" },
      new Country { ID = 77, Name = "France", Abbreviation = "FR" },
      new Country { ID = 78, Name = "French Guiana", Abbreviation = "GF" },
      new Country { ID = 79, Name = "French Polynesia", Abbreviation = "PF" },
      new Country { ID = 80, Name = "French Southern Territories", Abbreviation = "TF" },
      new Country { ID = 81, Name = "Gabon", Abbreviation = "GA" },
      new Country { ID = 82, Name = "Gambia", Abbreviation = "GM" },
      new Country { ID = 83, Name = "Georgia", Abbreviation = "GE" },
      new Country { ID = 84, Name = "Germany", Abbreviation = "DE" },
      new Country { ID = 85, Name = "Ghana", Abbreviation = "GH" },
      new Country { ID = 86, Name = "Gibraltar", Abbreviation = "GI" },
      new Country { ID = 87, Name = "Greece", Abbreviation = "GR" },
      new Country { ID = 88, Name = "Greenland", Abbreviation = "GL" },
      new Country { ID = 89, Name = "Grenada", Abbreviation = "GD" },
      new Country { ID = 90, Name = "Guadeloupe", Abbreviation = "GP" },
      new Country { ID = 91, Name = "Guam", Abbreviation = "GU" },
      new Country { ID = 92, Name = "Guatemala", Abbreviation = "GT" },
      new Country { ID = 93, Name = "Guernsey", Abbreviation = "GG" },
      new Country { ID = 94, Name = "Guinea", Abbreviation = "GN" },
      new Country { ID = 95, Name = "Guinea - bissau", Abbreviation = "GW" },
      new Country { ID = 96, Name = "Guyana", Abbreviation = "GY" },
      new Country { ID = 97, Name = "Haiti", Abbreviation = "HT" },
      new Country { ID = 98, Name = "Heard Island And Mcdonald Islands", Abbreviation = "HM" },
      new Country { ID = 99, Name = "Honduras", Abbreviation = "HN" },
      new Country { ID = 100, Name = "Hong Kong", Abbreviation = "HK" },
      new Country { ID = 101, Name = "Hungary", Abbreviation = "HU" },
      new Country { ID = 102, Name = "Iceland", Abbreviation = "IS" },
      new Country { ID = 103, Name = "India", Abbreviation = "IN" },
      new Country { ID = 104, Name = "Indonesia", Abbreviation = "ID" },
      new Country { ID = 105, Name = "Iran", Abbreviation = "IR" },
      new Country { ID = 106, Name = "Iraq", Abbreviation = "IQ" },
      new Country { ID = 107, Name = "Ireland", Abbreviation = "IE" },
      new Country { ID = 108, Name = "Isle Of Man", Abbreviation = "IM" },
      new Country { ID = 109, Name = "Israel", Abbreviation = "IL" },
      new Country { ID = 110, Name = "Italy", Abbreviation = "IT" },
      new Country { ID = 111, Name = "Jamaica", Abbreviation = "JM" },
      new Country { ID = 112, Name = "Japan", Abbreviation = "JP" },
      new Country { ID = 113, Name = "Jersey", Abbreviation = "JE" },
      new Country { ID = 114, Name = "Jordan", Abbreviation = "JO" },
      new Country { ID = 115, Name = "Kazakhstan", Abbreviation = "KZ" },
      new Country { ID = 116, Name = "Kenya", Abbreviation = "KE" },
      new Country { ID = 117, Name = "Kiribati", Abbreviation = "KI" },
      new Country { ID = 118, Name = "Korea (Democratic Peoples Republic)", Abbreviation = "KP" },
      new Country { ID = 119, Name = "Korea (Republic)", Abbreviation = "KR" },
      new Country { ID = 120, Name = "Kuwait", Abbreviation = "KW" },
      new Country { ID = 121, Name = "Kyrgyzstan", Abbreviation = "KG" },
      new Country { ID = 122, Name = "Lao (Peoples Democratic Republic)", Abbreviation = "LA" },
      new Country { ID = 123, Name = "Latvia", Abbreviation = "LV" },
      new Country { ID = 124, Name = "Lebanon", Abbreviation = "LB" },
      new Country { ID = 125, Name = "Lesotho", Abbreviation = "LS" },
      new Country { ID = 126, Name = "Liberia", Abbreviation = "LR" },
      new Country { ID = 127, Name = "Libya", Abbreviation = "LY" },
      new Country { ID = 128, Name = "Liechtenstein", Abbreviation = "LI" },
      new Country { ID = 129, Name = "Lithuania", Abbreviation = "LT" },
      new Country { ID = 130, Name = "Luxembourg", Abbreviation = "LU" },
      new Country { ID = 131, Name = "Macao", Abbreviation = "MO" },
      new Country { ID = 132, Name = "Macedonia", Abbreviation = "MK" },
      new Country { ID = 133, Name = "Madagascar", Abbreviation = "MG" },
      new Country { ID = 134, Name = "Malawi", Abbreviation = "MW" },
      new Country { ID = 135, Name = "Malaysia", Abbreviation = "MY" },
      new Country { ID = 136, Name = "Maldives", Abbreviation = "MV" },
      new Country { ID = 137, Name = "Mali", Abbreviation = "ML" },
      new Country { ID = 138, Name = "Malta", Abbreviation = "MT" },
      new Country { ID = 139, Name = "Marshall Islands", Abbreviation = "MH" },
      new Country { ID = 140, Name = "Martinique", Abbreviation = "MQ" },
      new Country { ID = 141, Name = "Mauritania", Abbreviation = "MR" },
      new Country { ID = 142, Name = "Mauritius", Abbreviation = "MU" },
      new Country { ID = 143, Name = "Mayotte", Abbreviation = "YT" },
      new Country { ID = 144, Name = "Mexico", Abbreviation = "MX" },
      new Country { ID = 145, Name = "Micronesia", Abbreviation = "FM" },
      new Country { ID = 146, Name = "Moldova", Abbreviation = "MD" },
      new Country { ID = 147, Name = "Monaco", Abbreviation = "MC" },
      new Country { ID = 148, Name = "Mongolia", Abbreviation = "MN" },
      new Country { ID = 149, Name = "Montenegro", Abbreviation = "ME" },
      new Country { ID = 150, Name = "Montserrat", Abbreviation = "MS" },
      new Country { ID = 151, Name = "Morocco", Abbreviation = "MA" },
      new Country { ID = 152, Name = "Mozambique", Abbreviation = "MZ" },
      new Country { ID = 153, Name = "Myanmar", Abbreviation = "MM" },
      new Country { ID = 154, Name = "Namibia", Abbreviation = "NA" },
      new Country { ID = 155, Name = "Nauru", Abbreviation = "NR" },
      new Country { ID = 156, Name = "Nepal", Abbreviation = "NP" },
      new Country { ID = 157, Name = "Netherlands", Abbreviation = "NL" },
      new Country { ID = 158, Name = "Netherlands Antilles", Abbreviation = "AN" },
      new Country { ID = 159, Name = "New Caledonia", Abbreviation = "NC" },
      new Country { ID = 160, Name = "New Zealand", Abbreviation = "NZ" },
      new Country { ID = 161, Name = "Nicaragua", Abbreviation = "NI" },
      new Country { ID = 162, Name = "Niger", Abbreviation = "NE" },
      new Country { ID = 163, Name = "Nigeria", Abbreviation = "NG" },
      new Country { ID = 164, Name = "Niue", Abbreviation = "NU" },
      new Country { ID = 165, Name = "Norfolk Island", Abbreviation = "NF" },
      new Country { ID = 166, Name = "Northern Mariana Islands", Abbreviation = "MP" },
      new Country { ID = 167, Name = "Norway", Abbreviation = "NO" },
      new Country { ID = 168, Name = "Oman", Abbreviation = "OM" },
      new Country { ID = 169, Name = "Pakistan", Abbreviation = "PK" },
      new Country { ID = 170, Name = "Palau", Abbreviation = "PW" },
      new Country { ID = 171, Name = "Palestinian Territory (Occupied)", Abbreviation = "PS" },
      new Country { ID = 172, Name = "Panama", Abbreviation = "PA" },
      new Country { ID = 173, Name = "Papua New Guinea", Abbreviation = "PG" },
      new Country { ID = 174, Name = "Paraguay", Abbreviation = "PY" },
      new Country { ID = 175, Name = "Peru", Abbreviation = "PE" },
      new Country { ID = 176, Name = "Philippines", Abbreviation = "PH" },
      new Country { ID = 177, Name = "Pitcairn", Abbreviation = "PN" },
      new Country { ID = 178, Name = "Poland", Abbreviation = "PL" },
      new Country { ID = 179, Name = "Portugal", Abbreviation = "PT" },
      new Country { ID = 180, Name = "Puerto Rico", Abbreviation = "PR" },
      new Country { ID = 181, Name = "Qatar", Abbreviation = "QA" },
      new Country { ID = 182, Name = "Reunion", Abbreviation = "RE" },
      new Country { ID = 183, Name = "Romania", Abbreviation = "RO" },
      new Country { ID = 184, Name = "Russian Federation", Abbreviation = "RU" },
      new Country { ID = 185, Name = "Rwanda", Abbreviation = "RW" },
      new Country { ID = 186, Name = "Saint Barthélemy", Abbreviation = "BL" },
      new Country { ID = 187, Name = "Saint Helena", Abbreviation = "SH" },
      new Country { ID = 188, Name = "Saint Kitts And Nevis", Abbreviation = "KN" },
      new Country { ID = 189, Name = "Saint Lucia", Abbreviation = "LC" },
      new Country { ID = 190, Name = "Saint Martin (French Part)", Abbreviation = "MF" },
      new Country { ID = 191, Name = "Saint Pierre And Miquelon", Abbreviation = "PM" },
      new Country { ID = 192, Name = "Saint Vincent And The Grenadines", Abbreviation = "VC" },
      new Country { ID = 193, Name = "Samoa", Abbreviation = "WS" },
      new Country { ID = 194, Name = "San Marino", Abbreviation = "SM" },
      new Country { ID = 195, Name = "Sao Tome And Principe", Abbreviation = "ST" },
      new Country { ID = 196, Name = "Saudi Arabia", Abbreviation = "SA" },
      new Country { ID = 197, Name = "Senegal", Abbreviation = "SN" },
      new Country { ID = 198, Name = "Serbia", Abbreviation = "RS" },
      new Country { ID = 199, Name = "Seychelles", Abbreviation = "SC" },
      new Country { ID = 200, Name = "Sierra Leone", Abbreviation = "SL" },
      new Country { ID = 201, Name = "Singapore", Abbreviation = "SG" },
      new Country { ID = 202, Name = "Sint Eustatius And Saba Bonaire", Abbreviation = "BQ" },
      new Country { ID = 203, Name = "Sint Maarten (Dutch Part)", Abbreviation = "SX" },
      new Country { ID = 204, Name = "Slovakia", Abbreviation = "SK" },
      new Country { ID = 205, Name = "Slovenia", Abbreviation = "SI" },
      new Country { ID = 206, Name = "Solomon Islands", Abbreviation = "SB" },
      new Country { ID = 207, Name = "Somalia", Abbreviation = "SO" },
      new Country { ID = 208, Name = "South Africa", Abbreviation = "ZA" },
      new Country { ID = 209, Name = "South Georgia And The South Sandwich Islands", Abbreviation = "GS" },
      new Country { ID = 210, Name = "South Sudan", Abbreviation = "SS" },
      new Country { ID = 211, Name = "Spain", Abbreviation = "ES" },
      new Country { ID = 212, Name = "Sri Lanka", Abbreviation = "LK" },
      new Country { ID = 213, Name = "Sudan", Abbreviation = "SD" },
      new Country { ID = 214, Name = "Suriname", Abbreviation = "SR" },
      new Country { ID = 215, Name = "Svalbard And Jan Mayen", Abbreviation = "SJ" },
      new Country { ID = 216, Name = "Swaziland", Abbreviation = "SZ" },
      new Country { ID = 217, Name = "Sweden", Abbreviation = "SE" },
      new Country { ID = 218, Name = "Switzerland", Abbreviation = "CH" },
      new Country { ID = 219, Name = "Syrian Arab Republic", Abbreviation = "SY" },
      new Country { ID = 220, Name = "Taiwan", Abbreviation = "TW" },
      new Country { ID = 221, Name = "Tajikistan", Abbreviation = "TJ" },
      new Country { ID = 222, Name = "Tanzania", Abbreviation = "TZ" },
      new Country { ID = 223, Name = "Thailand", Abbreviation = "TH" },
      new Country { ID = 224, Name = "Timor - leste", Abbreviation = "TL" },
      new Country { ID = 225, Name = "Togo", Abbreviation = "TG" },
      new Country { ID = 226, Name = "Tokelau", Abbreviation = "TK" },
      new Country { ID = 227, Name = "Tonga", Abbreviation = "TO" },
      new Country { ID = 228, Name = "T idad And Tobago", Abbreviation = "TT" },
      new Country { ID = 229, Name = "Tunisia", Abbreviation = "TN" },
      new Country { ID = 230, Name = "Turkey", Abbreviation = "TR" },
      new Country { ID = 231, Name = "Turkmenistan", Abbreviation = "TM" },
      new Country { ID = 232, Name = "Turks And Caicos Islands", Abbreviation = "TC" },
      new Country { ID = 233, Name = "Tuvalu", Abbreviation = "TV" },
      new Country { ID = 234, Name = "U.S. Virgin Islands", Abbreviation = "VI" },
      new Country { ID = 235, Name = "Uganda", Abbreviation = "UG" },
      new Country { ID = 236, Name = "Ukraine", Abbreviation = "UA" },
      new Country { ID = 237, Name = "United Arab Emirates", Abbreviation = "AE" },
      new Country { ID = 238, Name = "United Kingdom", Abbreviation = "GB" },
      new Country { ID = 239, Name = "United States", Abbreviation = "US" },
      new Country { ID = 240, Name = "United States Minor Outlying Islands", Abbreviation = "UM" },
      new Country { ID = 241, Name = "Uruguay", Abbreviation = "UY" },
      new Country { ID = 242, Name = "Uzbekistan", Abbreviation = "UZ" },
      new Country { ID = 243, Name = "Vanuatu", Abbreviation = "VU" },
      new Country { ID = 244, Name = "Vatican City", Abbreviation = "VA" },
      new Country { ID = 245, Name = "Venezuela", Abbreviation = "VE" },
      new Country { ID = 246, Name = "Viet Nam", Abbreviation = "VN" },
      new Country { ID = 247, Name = "Wallis And Futuna", Abbreviation = "WF" },
      new Country { ID = 248, Name = "Western Sahara", Abbreviation = "EH" },
      new Country { ID = 249, Name = "Yemen", Abbreviation = "YE" },
      new Country { ID = 250, Name = "Zambia", Abbreviation = "ZM" },
      new Country { ID = 251, Name = "Zimbabwe", Abbreviation = "ZW" }
    };
    private static readonly List<State> states = new()
    {
        new State { ID = 1, Name = "Alabama", Abbreviation = "AL" },
        new State { ID = 2, Name = "Alaska", Abbreviation = "AK" },
        new State { ID = 3, Name = "Arizona", Abbreviation = "AZ" },
        new State { ID = 4, Name = "Arkansas", Abbreviation = "AR" },
        new State { ID = 5, Name = "California", Abbreviation = "CA" },
        new State { ID = 6, Name = "Colorado", Abbreviation = "CO" },
        new State { ID = 7, Name = "Connecticut", Abbreviation = "CT" },
        new State { ID = 8, Name = "Delaware", Abbreviation = "DE" },
        new State { ID = 9, Name = "District of Columbia", Abbreviation = "DC" },
        new State { ID = 10, Name = "Florida", Abbreviation = "FL" },
        new State { ID = 11, Name = "Georgia", Abbreviation = "GA" },
        new State { ID = 12, Name = "Hawaii", Abbreviation = "HI" },
        new State { ID = 13, Name = "Idaho", Abbreviation = "ID" },
        new State { ID = 14, Name = "Illinois", Abbreviation = "IL" },
        new State { ID = 15, Name = "Indiana", Abbreviation = "IN" },
        new State { ID = 16, Name = "Iowa", Abbreviation = "IA" },
        new State { ID = 17, Name = "Kansas", Abbreviation = "KS" },
        new State { ID = 18, Name = "Kentucky", Abbreviation = "KY" },
        new State { ID = 19, Name = "Louisiana", Abbreviation = "LA" },
        new State { ID = 20, Name = "Maine", Abbreviation = "ME" },
        new State { ID = 21, Name = "Maryland", Abbreviation = "MD" },
        new State { ID = 22, Name = "Massachusetts", Abbreviation = "MA" },
        new State { ID = 23, Name = "Michigan", Abbreviation = "MI" },
        new State { ID = 24, Name = "Minnesota", Abbreviation = "MN" },
        new State { ID = 25, Name = "Mississippi", Abbreviation = "MS" },
        new State { ID = 26, Name = "Missouri", Abbreviation = "MO" },
        new State { ID = 27, Name = "Montana", Abbreviation = "MT" },
        new State { ID = 28, Name = "Nebraska", Abbreviation = "NE" },
        new State { ID = 29, Name = "Nevada", Abbreviation = "NV" },
        new State { ID = 30, Name = "New Hampshire", Abbreviation = "NH" },
        new State { ID = 31, Name = "New Jersey", Abbreviation = "NJ" },
        new State { ID = 32, Name = "New Mexico", Abbreviation = "NM" },
        new State { ID = 33, Name = "New York", Abbreviation = "NY" },
        new State { ID = 34, Name = "North Carolina", Abbreviation = "NC" },
        new State { ID = 35, Name = "North Dakota", Abbreviation = "ND" },
        new State { ID = 36, Name = "Ohio", Abbreviation = "OH" },
        new State { ID = 37, Name = "Oklahoma", Abbreviation = "OK" },
        new State { ID = 38, Name = "Oregon", Abbreviation = "OR" },
        new State { ID = 39, Name = "Pennsylvania", Abbreviation = "PA" },
        new State { ID = 40, Name = "Rhode Island", Abbreviation = "RI" },
        new State { ID = 41, Name = "South Carolina", Abbreviation = "SC" },
        new State { ID = 42, Name = "South Dakota", Abbreviation = "SD" },
        new State { ID = 43, Name = "Tennessee", Abbreviation = "TN" },
        new State { ID = 44, Name = "Texas", Abbreviation = "TX" },
        new State { ID = 45, Name = "Utah", Abbreviation = "UT" },
        new State { ID = 46, Name = "Vermont", Abbreviation = "VT" },
        new State { ID = 47, Name = "Virginia", Abbreviation = "VA" },
        new State { ID = 48, Name = "Washington", Abbreviation = "WA" },
        new State { ID = 49, Name = "West Virginia", Abbreviation = "WV" },
        new State { ID = 50, Name = "Wisconsin", Abbreviation = "WI" },
        new State { ID = 51, Name = "Wyoming", Abbreviation = "WY" },
        new State { ID = 52, Name = "Alberta", Abbreviation = null },
        new State { ID = 53, Name = "British Columbia", Abbreviation = null },
        new State { ID = 54, Name = "Manitoba", Abbreviation = null },
        new State { ID = 55, Name = "New Brunswick", Abbreviation = null },
        new State { ID = 56, Name = "Newfoundland", Abbreviation = null },
        new State { ID = 57, Name = "Northwest Territories", Abbreviation = null },
        new State { ID = 58, Name = "Nova Scotia", Abbreviation = null },
        new State { ID = 59, Name = "Nunavut", Abbreviation = null },
        new State { ID = 60, Name = "Ontario", Abbreviation = null },
        new State { ID = 61, Name = "Prince Edward Island", Abbreviation = null },
        new State { ID = 62, Name = "Québec", Abbreviation = null },
        new State { ID = 63, Name = "Saskatchewan", Abbreviation = "SK" },
        new State { ID = 64, Name = "Yukon Territory", Abbreviation = null },
        new State { ID = 65, Name = "Puerto Rico", Abbreviation = "PR" },
        new State { ID = 66, Name = "Northern Mariana Islands", Abbreviation = "MP" },
        new State { ID = 67, Name = "Guam", Abbreviation = "GU" },
        new State { ID = 68, Name = "AMERICAN SAMOA", Abbreviation = "AS" },
        new State { ID = 69, Name = "Virgin Islands", Abbreviation = "VI" },
        new State { ID = 70, Name = "Mexico", Abbreviation = null },
        new State { ID = 71, Name = "Armed Forces Africa", Abbreviation = "AE" },
        new State { ID = 72, Name = "Armed Forces Americas", Abbreviation = "AA" },
        new State { ID = 73, Name = "Armed Forces Pacific", Abbreviation = "AP" }
    };
    private static readonly List<AppSetting> appSettings = new()
    {
        new AppSetting
        {
            ID = 1,
            Name = "ShipmentDateOffset",
            Value = "0",
            Label = "Shipment Date Offset",
            Description = "Offset is being used to change the days in order shipment date",
            IsActive = true,
            IsDeleted = false
        }
    };

    public static User User { get => user; }
    public static List<Role> Roles { get => roles; }
    public static List<Gender> Genders { get => genders; }
    public static List<Country> Countries { get => countries; }
    public static List<State> States { get => states; }
    public static List<AppSetting> AppSettings { get => appSettings; }

}