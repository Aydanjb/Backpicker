import { useNavigate, useParams } from 'react-router';
import { Button } from '@/components/ui/button.tsx';
import {
	Table,
	TableBody,
	TableCell,
	TableHead,
	TableHeader,
	TableRow,
} from '@/components/ui/table.tsx';
import { formatPrice, formatWeight } from '@/lib/utils.ts';
import REILogo from '@/assets/REI_idsW56ot2c_0.svg';
import {useState} from "react";
import type {Kit} from "@/lib/types.tsx";

const mockData = [
	{
		id: 0,
		name: "Osprey Stratos 24 Pack - Men's",
		priceCents: 14999,
		weights: ['2 lbs. 15.2 oz.'],
		link: 'https://www.rei.com/product/C03941/osprey-stratos-24-pack-mens',
		image: 'https://www.rei.com/media/3c00cd80-0f87-400f-9fc0-0bc7d78d033b',
		ratings: [4.5, 1293],
	},
	{
		id: 0,
		name: "Osprey Stratos 24 Pack - Men's",
		priceCents: 14999,
		weights: ['2 lbs. 15.2 oz.'],
		link: 'https://www.rei.com/product/C03941/osprey-stratos-24-pack-mens',
		image: 'https://www.rei.com/media/3c00cd80-0f87-400f-9fc0-0bc7d78d033b',
		ratings: [2, 300],
	},
	{
		id: 0,
		name: "Osprey Stratos 24 Pack - Men's",
		priceCents: 14999,
		weights: ['2 lbs. 15.2 oz.'],
		link: 'https://www.rei.com/product/C03941/osprey-stratos-24-pack-mens',
		image: 'https://www.rei.com/media/3c00cd80-0f87-400f-9fc0-0bc7d78d033b',
		ratings: [5, 10],
	}
];

function Browse() {
	const { item } = useParams();
	const navigate = useNavigate();

	return (
		<div className="flex flex-col gap-4">
			<div className="flex items-center justify-between">
				<h2 className="font-2xl font-bold capitalize">
					Choose a {item}
				</h2>
				<Button variant="outline" onClick={() => navigate('/')}>
					Back to kit
				</Button>
			</div>

			<Table>
				<TableHeader>
					<TableRow>
						<TableHead />
						<TableHead>Name</TableHead>
						<TableHead>Weight</TableHead>
						<TableHead>Price</TableHead>
						<TableHead>Rating</TableHead>
						<TableHead>Where</TableHead>
					</TableRow>
				</TableHeader>
				<TableBody>
					<TableRow></TableRow>
				</TableBody>
			</Table>
		</div>
	);
}

export default Browse;
