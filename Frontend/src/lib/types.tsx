export interface Product {
	id: number;
	name: string;
	priceCents: number;
	weights?: Array<string> | string;
	link?: string;
	image?: string;
}

export interface Slot {
	id: number;
	label: string;
}

export type Kit = Record<string, Product | undefined>;
