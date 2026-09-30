using BookIt.Application.DTOs.WorkingHours;
using BookIt.Application.Validators.WorkingHours;
using FluentValidation.TestHelper;
using Xunit;

namespace BookIt.Tests.Validators
{
    public class WorkingHourDtoValidatorTests
    {
        private readonly WorkingHourDtoValidator _validator = new();

        // valid working day - each test changes only one thing
        private static WorkingHourDto ValidWorkingDay() => new()
        {
            DayOfWeek = 1,
            IsWorkingDay = true,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            PauseStart = new TimeOnly(12, 0),
            PauseEnd = new TimeOnly(12, 30)
        };

        // valid non-working day - all times are null
        private static WorkingHourDto NonWorkingDay() => new()
        {
            DayOfWeek = 0,
            IsWorkingDay = false
        };

        #region Valid

        [Fact]
        public void Validate_WorkingDayWithPause_HasNoErrors()
        {
            var result = _validator.TestValidate(ValidWorkingDay());

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_WorkingDayWithoutPause_HasNoErrors()
        {
            var dto = ValidWorkingDay();
            dto.PauseStart = null;
            dto.PauseEnd = null;

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_NonWorkingDayWithoutTimes_HasNoErrors()
        {
            var result = _validator.TestValidate(NonWorkingDay());

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_WorkingDayStartingAtMidnight_HasNoErrors()
        {
            // 00:00 is default(TimeOnly) - guards against using NotEmpty() instead of NotNull()
            var dto = ValidWorkingDay();
            dto.StartTime = new TimeOnly(0, 0);

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        #endregion

        #region DayOfWeek

        [Fact]
        public void Validate_DayOfWeekOutOfRange_HasErrorForDayOfWeek()
        {
            var dto = ValidWorkingDay();
            dto.DayOfWeek = 7;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.DayOfWeek);
        }

        #endregion

        #region Working hours

        [Fact]
        public void Validate_WorkingDayWithoutStartTime_HasErrorForStartTime()
        {
            var dto = ValidWorkingDay();
            dto.StartTime = null;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.StartTime);
        }

        [Fact]
        public void Validate_WorkingDayWithoutEndTime_HasErrorForEndTime()
        {
            var dto = ValidWorkingDay();
            dto.EndTime = null;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.EndTime);
        }

        [Fact]
        public void Validate_EndTimeBeforeStartTime_HasErrorForEndTime()
        {
            var dto = ValidWorkingDay();
            dto.StartTime = new TimeOnly(17, 0);
            dto.EndTime = new TimeOnly(9, 0);
            dto.PauseStart = null;
            dto.PauseEnd = null;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.EndTime);
        }

        #endregion

        #region Pause

        [Fact]
        public void Validate_PauseStartWithoutPauseEnd_HasErrorForPauseEnd()
        {
            var dto = ValidWorkingDay();
            dto.PauseEnd = null;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.PauseEnd);
        }

        [Fact]
        public void Validate_PauseEndWithoutPauseStart_HasErrorForPauseStart()
        {
            var dto = ValidWorkingDay();
            dto.PauseStart = null;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.PauseStart);
        }

        [Fact]
        public void Validate_PauseEndBeforePauseStart_HasErrorForPauseEnd()
        {
            var dto = ValidWorkingDay();
            dto.PauseStart = new TimeOnly(12, 30);
            dto.PauseEnd = new TimeOnly(12, 0);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.PauseEnd);
        }

        [Fact]
        public void Validate_PauseStartsBeforeWorkingHours_HasErrorForPauseStart()
        {
            var dto = ValidWorkingDay();
            dto.PauseStart = new TimeOnly(8, 0);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.PauseStart);
        }

        [Fact]
        public void Validate_PauseEndsAfterWorkingHours_HasErrorForPauseEnd()
        {
            var dto = ValidWorkingDay();
            dto.PauseEnd = new TimeOnly(18, 0);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.PauseEnd);
        }

        #endregion

        #region Non-working day

        [Fact]
        public void Validate_NonWorkingDayWithWorkingHours_HasErrorsForStartAndEndTime()
        {
            var dto = NonWorkingDay();
            dto.StartTime = new TimeOnly(9, 0);
            dto.EndTime = new TimeOnly(17, 0);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.StartTime);
            result.ShouldHaveValidationErrorFor(x => x.EndTime);
        }

        [Fact]
        public void Validate_NonWorkingDayWithPause_HasErrorsForPause()
        {
            var dto = NonWorkingDay();
            dto.PauseStart = new TimeOnly(12, 0);
            dto.PauseEnd = new TimeOnly(12, 30);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.PauseStart);
            result.ShouldHaveValidationErrorFor(x => x.PauseEnd);
        }

        #endregion
    }
}
