// jest-dom adds custom jest matchers for asserting on DOM nodes.
// allows you to do things like:
// expect(element).toHaveTextContent(/react/i)
// learn more: https://github.com/testing-library/jest-dom
import '@testing-library/jest-dom';

// Polyfill for TextEncoder/TextDecoder in Jest environment
import { TextEncoder, TextDecoder } from 'util';

global.TextEncoder = TextEncoder;
global.TextDecoder = TextDecoder;

// Mock HTMLCanvasElement for jsPDF compatibility in tests
Object.defineProperty(HTMLCanvasElement.prototype, 'getContext', {
  value: jest.fn(() => ({
    fillRect: jest.fn(),
    clearRect: jest.fn(),
    getImageData: jest.fn(() => ({ data: new Array(4) })),
    putImageData: jest.fn(),
    createImageData: jest.fn(() => []),
    setTransform: jest.fn(),
    drawImage: jest.fn(),
    save: jest.fn(),
    fillText: jest.fn(),
    restore: jest.fn(),
    beginPath: jest.fn(),
    moveTo: jest.fn(),
    lineTo: jest.fn(),
    closePath: jest.fn(),
    stroke: jest.fn(),
    translate: jest.fn(),
    scale: jest.fn(),
    rotate: jest.fn(),
    arc: jest.fn(),
    fill: jest.fn(),
    measureText: jest.fn(() => ({ width: 0 })),
    transform: jest.fn(),
    rect: jest.fn(),
    clip: jest.fn(),
  }))
});

// Mock HTMLCanvasElement for jsPDF in test environment
class MockCanvasRenderingContext2D {
  fillRect() {}
  clearRect() {}
  getImageData() {
    return {
      data: new Array(4)
    };
  }
  putImageData() {}
  createImageData() {
    return [];
  }
  setTransform() {}
  drawImage() {}
  save() {}
  fillText() {}
  restore() {}
  beginPath() {}
  moveTo() {}
  lineTo() {}
  closePath() {}
  stroke() {}
  translate() {}
  scale() {}
  rotate() {}
  arc() {}
  fill() {}
  measureText() {
    return { width: 0 };
  }
  transform() {}
  rect() {}
  clip() {}
}

global.HTMLCanvasElement.prototype.getContext = function(contextType) {
  if (contextType === '2d') {
    return new MockCanvasRenderingContext2D();
  }
  return null;
};

// Mock URL.createObjectURL for blob handling in tests
global.URL.createObjectURL = jest.fn(() => 'mocked-url');
global.URL.revokeObjectURL = jest.fn();
