import { ApiError } from './httpClient';

describe('ApiError', () => {
  it('prefers field validation messages', () => {
    const error = new ApiError(400, {
      title: 'Validation',
      errors: { Title: ['Title is required.'], PageCount: ['Too few.'] },
    });
    expect(error.message).toBe('Title is required. Too few.');
  });

  it('falls back to detail, then title, then status', () => {
    expect(new ApiError(409, { title: 'Book.StageSkipped', detail: 'One step at a time.' }).message).toBe(
      'One step at a time.',
    );
    expect(new ApiError(404, { title: 'Not Found' }).message).toBe('Not Found');
    expect(new ApiError(500, {}).message).toBe('Request failed (500).');
  });
});
