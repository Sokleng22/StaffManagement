import { render, screen } from '@testing-library/react';
import App from './App';

test('renders staff management system', () => {
  render(<App />);
  const titleElement = screen.getByText(/Staff Management System/i);
  expect(titleElement).toBeInTheDocument();
});
