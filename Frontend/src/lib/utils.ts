import type { Product } from '@/lib/types.tsx';

export { cn } from 'cn';

export function formatPrice(cents: number): string {
	return (cents / 100).toLocaleString('en-US', {
		style: 'currency',
		currency: 'USD',
	});
}

export function parseOunces(weights: Product['weights']): number {
	const w = Array.isArray(weights) ? weights[0] : weights;
	if (!w) return 0;
	const lbs = /([\d.]+)\s*lb/i.exec(w);
	const oz = /([\d.]+)\s*oz/i.exec(w);
	return (lbs ? parseFloat(lbs[1]) * 16 : 0) + (oz ? parseFloat(oz[1]) : 0);
}

export function formatWeight(oz: number): string {
	const lbs = Math.floor(oz / 16);
	const rem = Math.round((oz % 16) * 10) / 10;
	return lbs ? `${lbs} lb ${rem} oz` : `${rem} oz`;
}
