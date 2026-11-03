// Shared form-validation helpers so numeric/year fields behave identically
// across every modal and across browsers.
//
// Why this exists:
//   A native <input type="number"> behaves differently between browsers when a
//   user types alphanumeric characters. Chromium silently discards the invalid
//   characters, whereas Firefox lets them be typed and then reports the value as
//   an empty string (valueAsNumber === NaN). Combined with Vue's `.number`
//   modifier this meant invalid input could slip through without any feedback.
//   Centralising the checks guarantees consistent messaging everywhere.

/**
 * The earliest year accepted by any year field (console release year and
 * manufacturer founded year). Kept here so both modals stay in sync.
 */
export const MIN_YEAR = 1970;

/**
 * Returns true only when the supplied value is a real, finite number.
 * Rejects empty strings, null, undefined, NaN and Infinity.
 */
export function isValidNumber(value: unknown): boolean {
  if (value === null || value === undefined || value === '') {
    return false;
  }

  const parsed = typeof value === 'number' ? value : Number(value);
  return Number.isFinite(parsed);
}

/**
 * Validates a required numeric field within an (inclusive) range.
 * Returns an error message, or null when the value is valid.
 */
export function validateNumberInRange(
  value: unknown,
  label: string,
  min: number,
  max: number
): string | null {
  if (!isValidNumber(value)) {
    return `${label} must be a valid number.`;
  }

  const parsed = Number(value);
  if (parsed < min || parsed > max) {
    return `${label} must be between ${min} and ${max}.`;
  }

  return null;
}

/**
 * Validates a required year field. Years are just integers, so this builds on
 * validateNumberInRange but also rejects fractional values, keeping console and
 * manufacturer year validation perfectly consistent.
 */
export function validateYear(
  value: unknown,
  label: string,
  min: number,
  max: number
): string | null {
  const rangeError = validateNumberInRange(value, label, min, max);
  if (rangeError) {
    return rangeError;
  }

  if (!Number.isInteger(Number(value))) {
    return `${label} must be a whole year.`;
  }

  return null;
}
