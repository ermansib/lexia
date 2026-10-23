import { useMutation } from '@tanstack/react-query';

type FileRequest<T> = (input: T) => Promise<{ blob: Blob; fileName: string }>;

/** Runs a request that returns a file and saves it in the browser. */
export const useFileDownload = <T>(request: FileRequest<T>) =>
  useMutation({
    mutationFn: request,
    onSuccess: ({ blob, fileName }) => {
      const url = URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = fileName;
      link.click();
      URL.revokeObjectURL(url);
    },
  });
