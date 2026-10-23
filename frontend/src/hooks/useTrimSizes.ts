import { useQuery } from '@tanstack/react-query';
import { studioApi } from '@/services/api/studioApi';

export const useTrimSizes = () =>
  useQuery({ queryKey: ['trim-sizes'], queryFn: studioApi.trimSizes, staleTime: Infinity });
