import type { WordListInput } from '@/services/api/types';

export const SAMPLE_WORD_LISTS = [
  'Farm Animals: cow, horse, sheep, goat, chicken, duck, pig, donkey, rooster, turkey',
  'Ocean Life: whale, shark, dolphin, octopus, squid, coral, turtle, seal, starfish, jellyfish',
  'Garden: tulip, daisy, rose, lily, sunflower, orchid, violet, poppy, lavender, iris',
].join('\n');

/** Parses "Theme: word, word" lines into word lists, skipping blank or empty lines. */
export const parseWordLists = (text: string): WordListInput[] =>
  text
    .split('\n')
    .map((line) => {
      const [title, words = ''] = line.includes(':') ? line.split(/:(.*)/s) : ['Words', line];
      return {
        title: title.trim() || 'Words',
        words: words
          .split(',')
          .map((w) => w.trim())
          .filter(Boolean),
      };
    })
    .filter((list) => list.words.length > 0);
