using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace studentsapi.Tests.Helpers
{
    public class ModelValidationHelper
    {
        public static void ValidateModelState(ControllerBase controller, object model)
        {
            // Clear the ModelState
            controller.ModelState.Clear();
            // Validate the model
            if (model == null)
                return;
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);
            // Add validation errors to ModelState if any
            if (!isValid)
            {
                foreach (var validationResult in validationResults)
                {
                    foreach (var memberName in validationResult.MemberNames)
                    {
                        controller.ModelState.AddModelError(memberName, validationResult.ErrorMessage);
                    }
                }
            }
        }
    }
}
