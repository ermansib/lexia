import { parseWordLists } from './wordLists';

describe('parseWordLists', () => {
  it('reads a theme and comma-separated words from each line', () => {
    expect(parseWordLists('Farm: cow, pig ,hen\nSea: whale')).toEqual([
      { title: 'Farm', words: ['cow', 'pig', 'hen'] },
      { title: 'Sea', words: ['whale'] },
    ]);
  });

  it('uses a default title and skips lines with no words', () => {
    expect(parseWordLists('apple, pear\n\nEmpty:   ')).toEqual([
      { title: 'Words', words: ['apple', 'pear'] },
    ]);
  });
});
