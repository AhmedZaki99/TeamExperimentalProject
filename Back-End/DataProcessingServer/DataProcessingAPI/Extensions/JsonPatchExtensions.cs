using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace DataProcessingAPI
{
    /// <summary>
    /// Extensions for <see cref="JsonPatchDocument{T}"/>
    /// </summary>
    public static class JsonPatchExtensions
    {

        #region Document Application

        /// <summary>
        /// Applies JSON patch operations on object and logs errors in <see cref="ModelStateDictionary"/>.
        /// </summary>
        /// <param name="patchDoc">The <see cref="JsonPatchDocument{T}"/>.</param>
        /// <param name="objectToApplyTo">The entity on which <see cref="JsonPatchDocument{T}"/> is applied.</param>
        /// <param name="modelState">The <see cref="ModelStateDictionary"/> to add errors.</param>
        public static bool TryApplyTo<T>(this JsonPatchDocument<T> patchDoc, T objectToApplyTo, ModelStateDictionary modelState) where T : class
        {
            ArgumentNullException.ThrowIfNull(patchDoc);
            ArgumentNullException.ThrowIfNull(objectToApplyTo);
            ArgumentNullException.ThrowIfNull(modelState);

            if (patchDoc.ValidateDocument(modelState))
            {
                patchDoc.ApplyTo(objectToApplyTo, err => modelState.TryAddModelError("JSON Patch", err.ErrorMessage));

                if (modelState.IsValid)
                {
                    ValidateObject(objectToApplyTo, modelState); 
                }
            }
            return modelState.IsValid;
        } 
        
        /// <summary>
        /// Applies JSON patch operations on object and logs errors in <see cref="ModelStateDictionary"/>,
        /// after checking against a set of restricted path segments to edit.
        /// </summary>
        /// <param name="patchDoc">The <see cref="JsonPatchDocument{T}"/>.</param>
        /// <param name="objectToApplyTo">The entity on which <see cref="JsonPatchDocument{T}"/> is applied.</param>
        /// <param name="modelState">The <see cref="ModelStateDictionary"/> to add errors.</param>
        /// <param name="restricted">The collection of restricted path segments.</param>
        public static bool TryApplyTo<T>(this JsonPatchDocument<T> patchDoc, T objectToApplyTo, ModelStateDictionary modelState, IEnumerable<string> restricted) where T : class
        {
            ArgumentNullException.ThrowIfNull(patchDoc);
            ArgumentNullException.ThrowIfNull(objectToApplyTo);
            ArgumentNullException.ThrowIfNull(modelState);

            bool isDocValid = patchDoc.ValidateDocument(modelState);

            if (isDocValid && CheckRestrictedSegment(patchDoc.Operations, restricted) is string restrictedSeg)
            {
                modelState.TryAddModelError("JSON Patch", $"The target location specified by path segment '{restrictedSeg}' is immutable.");
            }

            if (modelState.IsValid)
            {
                patchDoc.ApplyTo(objectToApplyTo, err => modelState.TryAddModelError("JSON Patch", err.ErrorMessage));

                ValidateObject(objectToApplyTo, modelState);
            }
            return modelState.IsValid;
        }

        #endregion


        #region Helper Methods

        #region Validation

        private static bool ValidateDocument<T>(this JsonPatchDocument<T> patchDoc, ModelStateDictionary modelState) where T : class
        {
            if (patchDoc.Operations.Any(o => o.op is null || o.path is null))
            {
                modelState.TryAddModelError("JSON Patch", "All operations must have a valid 'op' and 'path' values.");
            }
            else if (patchDoc.Operations.Any(o => o.OperationType is OperationType.Move or OperationType.Copy && o.from is null))
            {
                modelState.TryAddModelError("JSON Patch", "Move and copy operations must have a valid 'from' value.");
            }
            return modelState.IsValid;
        }

        private static bool ValidateObject<T>(T objectToValidate, ModelStateDictionary modelState) where T : class
        {
            List<ValidationResult> results = new();
            ValidationContext context = new(objectToValidate);

            Validator.TryValidateObject(objectToValidate, context, results, true);
            foreach (var error in results)
            {
                modelState.TryAddModelError(error.MemberNames.First(), error.ErrorMessage ?? "Invalid value.");
            }

            return modelState.IsValid;
        }

        #endregion

        #region Restriction Check

        private static string? CheckRestrictedSegment<T>(List<Operation<T>> operations, IEnumerable<string> restrictions) where T : class
        {
            return operations.FirstOrDefault(o => o.IsRestricted(restrictions))?.GetFirstSegment();
        }

        private static bool IsRestricted<T>(this Operation<T> operation, IEnumerable<string> restrictions) where T : class
        {
            return operation.OperationType != OperationType.Test && IsSegmentRestricted(operation.GetFirstSegment(), restrictions);
        }

        private static bool IsSegmentRestricted(string segment, IEnumerable<string> restrictions)
        {
            return restrictions.Any(x => string.Equals(x, segment, StringComparison.OrdinalIgnoreCase));
        }

        private static string GetFirstSegment<T>(this Operation<T> operation) where T : class
        {
            return operation.path.Split('/')[1];
        }

        #endregion

        #endregion

    }
}
