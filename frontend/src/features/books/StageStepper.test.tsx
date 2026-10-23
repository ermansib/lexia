import { fireEvent, render, screen } from '@testing-library/react';
import { StageStepper } from './StageStepper';

describe('StageStepper', () => {
  it('lets authors open steps up to one past the current stage', () => {
    const onView = vi.fn();
    render(<StageStepper current="Research" viewing="Research" onView={onView} />);

    expect(screen.getByRole('button', { name: /Idea/ })).toBeEnabled();
    expect(screen.getByRole('button', { name: /Create/ })).toBeEnabled();
    expect(screen.getByRole('button', { name: /Design/ })).toBeDisabled();
    expect(screen.getByRole('button', { name: /Research/ })).toHaveAttribute('aria-current', 'step');

    fireEvent.click(screen.getByRole('button', { name: /Create/ }));
    expect(onView).toHaveBeenCalledWith('Create');
  });
});
